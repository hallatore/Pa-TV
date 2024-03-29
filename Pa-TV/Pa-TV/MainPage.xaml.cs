﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Pa_TV.Models;
using Pa_TV.Service;
using Pa_TV.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Search;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
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
        private MainPageViewModel viewModel;
        private DispatcherTimer timer;

        public MainPage()
        {
            InitializeComponent();

            SearchPane.GetForCurrentView().ShowOnKeyboardInput = true;
            SearchPane.GetForCurrentView().QuerySubmitted += MainPageQuerySubmitted;

            ApplicationViewStates.CurrentStateChanged += ApplicationViewStates_CurrentStateChanged;

            ApplicationData.Current.DataChanged += Current_DataChanged;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(20);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        void ApplicationViewStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            CheckScrolling();
        }

        async void Current_DataChanged(ApplicationData sender, object args)
        {
            if (viewModel != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var channels = App.GetChannelsOrDefault(sender);
                    var changed = CheckIfChanged(channels);

                    if (changed)
                    {
                        viewModel.Channels = channels;
                        await LoadChannels();
                    }
                    else
                    {
                        UpdateFavorites();
                    }
                });
            }
        }

        private bool CheckIfChanged(IEnumerable<string> channels)
        {
            var changed = viewModel.Channels.Count() != channels.Count();

            if (!changed)
            {
                foreach (var channel in channels)
                {
                    changed = changed || viewModel.Channels.All(s => s != channel);
                }
            }

            if (!changed)
            {
                foreach (var channel in viewModel.Channels)
                {
                    changed = changed || channels.All(s => s != channel);
                }
            }
            return changed;
        }

        void Timer_Tick(object sender, object e)
        {
            if (viewModel == null) return;

            DrawCurrentTimeLine(viewModel.Start, viewModel.End);
            var now = DateTime.Now;

            foreach (var channel in viewModel.ChannelList)
            {
                foreach (var eventItem in channel.Events)
                {
                    if (eventItem.End < now)
                        eventItem.NotifyPropertyChanged("Ended");
                }
            }
        }

        protected async override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (viewModel == null)
            {
                viewModel = new MainPageViewModel
                {
                    Start = DateTime.Today.AddHours(5),
                    Channels = App.GetChannelsOrDefault(),
                    Favorites = App.GetFavoritesOrDefault()
                };

                DataContext = viewModel;

                DaySelectorItems.ItemsSource = new[]
                {
                    viewModel.Start,
                    viewModel.Start.AddDays(1),
                    viewModel.Start.AddDays(2),
                    viewModel.Start.AddDays(3),
                    viewModel.Start.AddDays(4),
                    viewModel.Start.AddDays(5),
                    viewModel.Start.AddDays(6)
                };

                await LoadChannels();
            }

            CheckScrolling();
        }

        private async Task LoadChannels()
        {
            ProgressRingControl.IsActive = true;
            Scroller.Opacity = 0;
            
            ScrollerContainer.Children.Clear();
            TimeHeaders.Children.Clear();
            ChannelsStackPanel.Children.Clear();
            SearchStatusText.Text = string.Empty;
            SearchHintContainer.Children.Clear();
            _currentTimeLine = null;
            viewModel.End = DateTime.MinValue;

            var start = viewModel.Start;
            var current = start.AddMinutes(30);

            var er = new EventRetriever();
            viewModel.ChannelList = await er.GetEventsForDateAsync(viewModel.Start, viewModel.Channels);

            foreach (var channel in viewModel.ChannelList)
            {
                var ci = channel.Events.Max(ev => ev.End);
                if (ci > viewModel.End)
                {
                    viewModel.End = ci;
                }
            }

            DrawTimeLines(current, viewModel.End, TimeWidth);
            DrawChannels(viewModel.ChannelList, start);
            DrawCurrentTimeLine(start, viewModel.End);
            UpdateFavorites();
            Scroller.Opacity = 1;
            UpdateLayout();
            CheckScrolling();
            TimeHeaders.Width = ScrollerContainer.ActualWidth;
            ScrollToCurrentTime(start, viewModel.End);

            ProgressRingControl.IsActive = false;
        }

        private void CheckScrolling()
        {
            if (Scroller.ComputedVerticalScrollBarVisibility == Visibility.Collapsed)
                Scroller.VerticalScrollMode = ScrollMode.Disabled;
            else
                Scroller.VerticalScrollMode = ScrollMode.Auto;
        }

        private Grid _currentTimeLine;
        private Event _activeEventItem;

        private void DrawCurrentTimeLine(DateTime start, DateTime end)
        {
            if (DateTime.Now >= start && DateTime.Now <= end)
            {
                var now = DateTime.Now - start;
                var leftOffset = (now.TotalMinutes/30)*TimeWidth;

                if (_currentTimeLine == null)
                {
                    _currentTimeLine = new Grid
                    {
                        Background = (SolidColorBrush) Application.Current.Resources["AppBrush"],
                        Width = 1,
                        Opacity = 0.7,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    ScrollerContainer.Children.Add(_currentTimeLine);
                }

                _currentTimeLine.Margin = new Thickness(leftOffset, 0, 0, 0);
            }
        }

        private void ScrollToCurrentTime(DateTime start, DateTime end)
        {
            if (DateTime.Now >= start && DateTime.Now <= end)
            {
                var now = DateTime.Now - start;
                var leftOffset = (now.TotalMinutes / 30) * TimeWidth;
                Scroller.ScrollToHorizontalOffset(leftOffset - TimeWidth);
            }
        }

        private void DrawChannels(IEnumerable<Channel> result, DateTime start)
        {
            var sp = new StackPanel();
            sp.HorizontalAlignment = HorizontalAlignment.Left;

            foreach (var channel in result)
            {
                var events = new Grid();
                events.HorizontalAlignment = HorizontalAlignment.Left;
                events.Height = RowHeight;
                events.Margin = new Thickness(0, 0, 0, -1);

                foreach (var eventItem in channel.Events)
                {
                    if (eventItem.Start < start) continue;

                    var button = GetEvent(eventItem, start);
                    button.Click += ShowEventInfo_Click;
                    events.Children.Add(button);
                }

                sp.Children.Add(events);

                var channelElement = new Button
                {
                    DataContext = new { RowHeight = RowHeight, Channel = channel },
                    Style = (Style)Resources["ChannelButtonStyle"],
                };

                channelElement.Click += ChannelElementOnClick;
                ChannelsStackPanel.Children.Add(channelElement);
            }

            ScrollerContainer.Children.Add(sp);
        }

        void MainPageQuerySubmitted(SearchPane sender, SearchPaneQuerySubmittedEventArgs args)
        {
            Search(args.QueryText);
        }

        public void Search(string query)
        {
            if (viewModel == null || string.IsNullOrWhiteSpace(query)) return;

            SearchHintContainer.Children.Clear();
            var hintWidth = SearchHintContainer.ActualWidth;
            var eventsWidth = ScrollerContainer.ActualWidth;
            var start = viewModel.Start;
            var found = 0;
            Event firstItem = null;

            foreach (var channel in viewModel.ChannelList)
            {
                foreach (var eventItem in channel.Events)
                {
                    if ((Regex.IsMatch(eventItem.Title, query, RegexOptions.IgnoreCase) || 
                        Regex.IsMatch(eventItem.Description, query, RegexOptions.IgnoreCase)) && 
                        eventItem.Start > start)
                    {
                        if ((firstItem == null || firstItem.Start > eventItem.Start) && eventItem.End >= DateTime.Now)
                            firstItem = eventItem;

                        found++;
                        eventItem.HighLight = true;

                        var leftTime = eventItem.Start - start;
                        var leftMargin = (leftTime.TotalMinutes/30)*TimeWidth;

                        var widthTime = eventItem.End - eventItem.Start;
                        var width = (widthTime.TotalMinutes/30)*TimeWidth;

                        leftMargin = leftMargin/eventsWidth*hintWidth;
                        width = width/eventsWidth*hintWidth;

                        SearchHintContainer.Children.Add(new Grid
                        {
                            Background = (Brush) Application.Current.Resources["AppBrush"],
                            Margin = new Thickness(leftMargin, 0, 0, 0),
                            Width = width,
                            HorizontalAlignment = HorizontalAlignment.Left
                        });
                    }
                    else
                        eventItem.HighLight = false;
                }
            }

            if (firstItem != null)
            {
                var leftTime = firstItem.Start - start;
                var leftMargin = (leftTime.TotalMinutes / 30) * TimeWidth;
                Scroller.ScrollToHorizontalOffset(leftMargin - TimeWidth);
            }

            SearchStatusText.Text = string.Format("Resultater for \"{0}\" ({1})", query, found);
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchStatusText.Text = string.Empty;
            SearchHintContainer.Children.Clear();

            foreach (var channel in viewModel.ChannelList)
            {
                foreach (var eventItem in channel.Events)
                {
                    eventItem.HighLight = false;
                }
            }
        }

        private void ChannelElementOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var button = (Button) routedEventArgs.OriginalSource;
            var dctx = button.DataContext;
            Channel channel = ((dynamic) dctx).Channel;

            Frame.Navigate(typeof(ChannelDetailsPage), channel);
        }

        private void DrawTimeLines(DateTime current, DateTime end, int marginLeft)
        {
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
                    time.Margin = new Thickness(marginLeft - 25, 0, 0, 0);
                }

                TimeHeaders.Children.Add(time);

                ScrollerContainer.Children.Add(new Grid
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
        }

        async void ShowEventInfo_Click(object sender, RoutedEventArgs e)
        {
            _activeEventItem = (Event) ((FrameworkElement) sender).DataContext;

            string favoriteButton = "Favoritt";

            if (viewModel.Favorites.Any(s => s == _activeEventItem.Title))
                favoriteButton = "Fjern favoritt";


            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
            var dialog = new MessageDialog(string.Format("{0:HH:mm} - {1:HH:mm}\r\n\r\n{2}", _activeEventItem.Start, _activeEventItem.End, _activeEventItem.Description), _activeEventItem.Title);
            dialog.Commands.Add(new UICommand(favoriteButton, command =>
            {
                DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
                var favList = viewModel.Favorites.ToList();

                if (!viewModel.Favorites.Any(s => s == _activeEventItem.Title))
                    favList.Add(_activeEventItem.Title);
                else
                    favList.Remove(_activeEventItem.Title);

                    viewModel.Favorites = favList;
                    App.SaveFavorites(favList);
            }));
            dialog.Commands.Add(new UICommand("Del", OnCLickShare));
            dialog.Commands.Add(new UICommand("Lukk", command =>
            {
                DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
            }));

            dialog.DefaultCommandIndex = 2;

            await dialog.ShowAsync();
        }

        private void UpdateFavorites()
        {
            foreach (var channel in viewModel.ChannelList)
            {
                foreach (var eventItem in channel.Events)
                {
                    eventItem.Favorite = viewModel.Favorites.Any(s => s == eventItem.Title);
                }
            }
        }

        private void OnCLickShare(IUICommand command)
        {
            DataTransferManager.ShowShareUI();
        }

        void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (_activeEventItem == null)
                return;

            var request = args.Request;

            request.Data.Properties.Title = _activeEventItem.Title;
            request.Data.Properties.Description = _activeEventItem.Description;

            request.Data.SetText(string.Format(
                "På TV! {0} ({1:HH:mm} - {2:HH:mm, dd.MM.yyyy})",
                _activeEventItem.Title, _activeEventItem.Start, _activeEventItem.End));

            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
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
                Style = (Style)Application.Current.Resources["EventButtonStyle"]
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

        private void ShowChannelsPage_Click(object sender, RoutedEventArgs e)
        {
            bottomAppBar.IsOpen = false;
            Frame.Navigate(typeof(ChannelsPage));
        }

        private void ShowHideDaySelection_Click(object sender, RoutedEventArgs e)
        {
            if (DaySelectorGrid.Visibility == Visibility.Collapsed)
                DaySelectorGrid.Visibility = Visibility.Visible;
            else
                DaySelectorGrid.Visibility = Visibility.Collapsed;
        }

        private void DaySelect_Click(object sender, RoutedEventArgs e)
        {
            DaySelectorGrid.Visibility = Visibility.Collapsed;
            var date = (DateTime) ((FrameworkElement) sender).DataContext;
            viewModel.Start = date.AddHours(5);
            LoadChannels();
        }

        private void Header_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel == null) return;

            ScrollToCurrentTime(viewModel.Start, viewModel.End);
        }
    }
}
