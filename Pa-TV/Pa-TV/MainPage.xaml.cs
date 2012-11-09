using System;
using System.Linq;
using Pa_TV.Models;
using Pa_TV.Service;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Pa_TV
{
    public sealed partial class MainPage
    {
        private const int TimeWidth = 300;
        private const int RowHeight = 55;

        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var start = DateTime.Today.AddHours(5);

            if (DateTime.Now.Hour < 5)
                start = start.AddDays(-1);

            var end = start.AddDays(1);
            var current = start;
            var marginLeft = 0;

            while (current < end)
            {
                TimeHeaders.Children.Add(new TextBlock
                {
                    Style = (Style)Application.Current.Resources["GroupHeaderTextStyle"],
                    Text = current.ToString("HH:mm"),
                    Margin = new Thickness(marginLeft,0,0,0)
                });

                marginLeft += TimeWidth;
                current = current.AddMinutes(30);
            }

            var er = new EventRetriever();

            var result = await er.GetEventsTodayAsync();

            foreach (var channel in result.Where(c => c.Name.StartsWith("TV")))
            {
                var events = new Grid();
                events.HorizontalAlignment = HorizontalAlignment.Left;
                events.Height = RowHeight;
                events.Margin = new Thickness(0,0,0,-1);

                foreach (var eventItem in channel.Events)
                {
                    if (eventItem.Start >= start)
                    events.Children.Add(GetEvent(eventItem, start));
                }

                EventsStackPanel.Children.Add(events);

                ChannelsStackPanel.Children.Add(new Image { Source = new BitmapImage(channel.LogoUrl), Height = RowHeight - 1, HorizontalAlignment = HorizontalAlignment.Right });
            }

            var now = DateTime.Now - start;
            var leftOffset = (now.TotalMinutes/30)*TimeWidth;

            Scroller.ScrollToHorizontalOffset(leftOffset);
            ScrollerContainer.Children.Add(new Grid
            {
                Background = new SolidColorBrush(Colors.Yellow),
                Width = 5,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(leftOffset, 0, 0, 0)
            });
        }

        private UIElement GetEvent(Event eventItem, DateTime start)
        {
            var leftTime = eventItem.Start - start;
            var leftMargin = (leftTime.TotalMinutes / 30) * TimeWidth;

            var widthTime = eventItem.End - eventItem.Start;
            var width = (widthTime.TotalMinutes / 30) * TimeWidth;

            return new Button
            {
                DataContext = eventItem,
                Margin = new Thickness(leftMargin, 0, 0, 0),
                Width = width + 1,
                Style = (Style)Resources["EventButtonStyle"]
            };
        }

        private void Scroller_ViewChanged_1(object sender, ScrollViewerViewChangedEventArgs e)
        {
            HeaderScroller.ScrollToHorizontalOffset(Scroller.HorizontalOffset);
            ChannelsScroller.ScrollToVerticalOffset(Scroller.VerticalOffset);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Scroller.ScrollToVerticalOffset(Math.Max(Scroller.VerticalOffset - 200, 0));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset + 200);
        }

        private void ChannelsScroller_ViewChanged_1(object sender, ScrollViewerViewChangedEventArgs e)
        {
            Scroller.ScrollToVerticalOffset(ChannelsScroller.VerticalOffset);
        }
    }
}
