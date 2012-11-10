using System;
using System.Linq;
using Pa_TV.Models;
using Pa_TV.Service;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Pa_TV
{
    public sealed partial class MainPage
    {
        private const int TimeWidth = 250;
        private const int RowHeight = 60;

        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var start = DateTime.Today.AddHours(5);

            var end = start.AddDays(1);
            var marginLeft = TimeWidth;
            var current = start.AddMinutes(30);

            while (current < end)
            {
                var time = new TextBlock
                {
                    Style = (Style) Application.Current.Resources["ItemTextStyle"],
                    Text = current.ToString("HH:mm"),
                    Margin = new Thickness(marginLeft - 15, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                if (current.Minute == 0)
                {
                    time.Style = (Style) Application.Current.Resources["GroupHeaderTextStyle"];
                    time.Margin = new Thickness(marginLeft -25, 0, 0, 0);
                }

                TimeHeaders.Children.Add(time);

                ScrollerContainer.Children.Insert(0,new Grid
                {
                    Background = new SolidColorBrush(Colors.White),
                    Width = 1,
                    Opacity = 0.05,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(marginLeft, 0, 0, 0)
                });

                marginLeft += TimeWidth;
                current = current.AddMinutes(30);
            }

            var er = new EventRetriever();

            var result = await er.GetEventsTodayAsync(new int[] { 10, 11, 12, 9, 15 });

            foreach (var channel in result)
            {
                var events = new Grid();
                events.HorizontalAlignment = HorizontalAlignment.Left;
                events.Height = RowHeight;
                events.Margin = new Thickness(0,0,0,-1);

                foreach (var eventItem in channel.Events)
                {
                    if (eventItem.Start < start) continue;

                    var button = GetEvent(eventItem, start);
                    button.Click += button_Click;
                    events.Children.Add(button);
                }

                EventsStackPanel.Children.Add(events);

                var channelElement = new Border
                {
                    BorderThickness = new Thickness(0, 1, 0, 1),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60)),
                    Height = RowHeight,
                    Margin = new Thickness(0,0,0,-1),
                    Child = new Image
                    {
                        Source = new BitmapImage(channel.LogoUrl),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0,0,0,0)
                    }
                };
                ChannelsStackPanel.Children.Add(channelElement);
            }

            if (DateTime.Now >= start && DateTime.Now <= end)
            {
                var now = DateTime.Now - start;
                var leftOffset = (now.TotalMinutes/30)*TimeWidth;

                Scroller.ScrollToHorizontalOffset(leftOffset);
                ScrollerContainer.Children.Add(new Grid
                {
                    Background = new SolidColorBrush(Colors.Yellow),
                    Width = 1,
                    Opacity = 0.7,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(300, 0, 0, 0)
                });
            }
        }

        async void button_Click(object sender, RoutedEventArgs e)
        {
            var eventItem = (Event) ((FrameworkElement) sender).DataContext;
            await new MessageDialog(string.Format("{0:HH:mm} - {1:HH:mm}\r\n\r\n{2}", eventItem.Start, eventItem.End, eventItem.Description), eventItem.Title).ShowAsync();
        }

        private Button GetEvent(Event eventItem, DateTime start)
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
