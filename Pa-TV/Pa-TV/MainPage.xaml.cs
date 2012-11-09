using System;
using Pa_TV.Models;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Pa_TV
{
    public sealed partial class MainPage
    {
        private const int TimeWidth = 150;

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var start = DateTime.Today.AddHours(5);
            var now = start.AddHours(7) - start;
            Scroller.ScrollToHorizontalOffset((now.TotalMinutes / 30) * TimeWidth);
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var start = DateTime.Today.AddHours(5);
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

            var events = new Grid();
            events.HorizontalAlignment = HorizontalAlignment.Left;
            events.Height = 70;

            var border = GetEvent(new Event{ Start = DateTime.Now, Title = "Tore sin film"});

            events.Children.Add(border);

            events.Children.Add(new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(TimeWidth * 4, 0, 0, 0),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 45, 45, 45)),
                Width = TimeWidth * 1 + 1
            });

            events.Children.Add(new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(TimeWidth * 5, 0, 0, 0),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 45, 45, 45)),
                Width = TimeWidth * 1 + 1
            });

            EventsStackPanel.Children.Add(events);
        }

        private UIElement GetEvent(Models.Event eventItem)
        {
            return new Button
            {
                DataContext = eventItem,
                Margin = new Thickness(0, 0, 0, 0),
                Width = TimeWidth*3 + 1,
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
    }
}
