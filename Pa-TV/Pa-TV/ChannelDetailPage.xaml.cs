using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Pa_TV.Extensions;
using Pa_TV.Models;
using Pa_TV.Service;
using Pa_TV.ViewModels;
using Windows.ApplicationModel.Search;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Pa_TV
{
    public sealed partial class ChannelDetailsPage
    {
        private const int TimeWidth = 250;
        private const int RowHeight = 60;

        private ChannelDetailsPageViewModel viewModel;

        public ChannelDetailsPage()
        {
            InitializeComponent();

            SearchPane.GetForCurrentView().ShowOnKeyboardInput = true;
            SearchPane.GetForCurrentView().QuerySubmitted += ChannelDetailsPage_QuerySubmitted;
        }

        void ChannelDetailsPage_QuerySubmitted(SearchPane sender, SearchPaneQuerySubmittedEventArgs args)
        {
            Search(args.QueryText);
        }

        public void Search(string query)
        {
            if (viewModel == null || string.IsNullOrWhiteSpace(query)) return;

            foreach (var channel in viewModel.EventsForDate)
            {
                foreach (var eventItem in channel.Events)
                {
                    if (Regex.IsMatch(eventItem.Title, query, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(eventItem.Description, query, RegexOptions.IgnoreCase))
                    {
                        eventItem.HighLight = true;
                    }
                    else
                        eventItem.HighLight = false;
                }
            }

            //SearchStatusText.Text = string.Format("Resultater for \"{0}\" ({1})", query, found);
        }

        protected async override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (viewModel == null)
            {
                var channel = (Channel) e.Parameter;
                viewModel = new ChannelDetailsPageViewModel(channel);

                DataContext = viewModel;
            }

            await viewModel.LoadData();
        }

        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //var group = new ChannelGroup();
            //var channels = new List<string>();

            //foreach (var item in itemGridView.Items)
            //{
            //    if (item is Channel && itemGridView.SelectedItems.OfType<Channel>().Any(c => c.Id == ((Channel)item).Id))
            //    {
            //        channels.Add(((Channel)item).Id);
            //    }
            //}

            //group.ChannelIds = channels;
            //ApplicationData.Current.RoamingSettings.Values["Channels"] = group.ToJson();
        }

        private async Task LoadChannels()
        {
            //var er = new EventRetriever();
            //var result = await er.GetEventsTodayAsync();
            
            //var channels = result.Select(c => new Channel
            //{
            //    Id = c.Id,
            //    Name = c.Name,
            //    LogoUrl = c.LogoUrl
            //}).ToList();

            //var selectedChannels = channels.Where(c => _viewModel.Channels.Any(c2 => c2 == c.Id));

            //itemGridView.ItemsSource = channels;
            //UpdateLayout();

            //foreach (var selectedChannel in selectedChannels)
            //{
            //    itemGridView.SelectedItems.Add(selectedChannel);
            //}
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var eventItem = (Event)((FrameworkElement)sender).DataContext;
            var dialog = new MessageDialog(string.Format("{0:HH:mm} - {1:HH:mm}\r\n\r\n{2}", eventItem.Start, eventItem.End, eventItem.Description), eventItem.Title);
            dialog.Commands.Add(new UICommand("Lukk"));
            await dialog.ShowAsync();
        }
    }
}
