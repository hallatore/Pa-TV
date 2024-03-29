﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pa_TV.Extensions;
using Pa_TV.Models;
using Pa_TV.Service;
using Pa_TV.ViewModels;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Pa_TV
{
    public sealed partial class ChannelsPage
    {
        private const int TimeWidth = 250;
        private const int RowHeight = 60;
        private ChannelsPageViewModel viewModel;

        public ChannelsPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (viewModel == null)
            {
                viewModel = new ChannelsPageViewModel
                {
                    Channels = App.GetChannelsOrDefault()
                };

                await LoadChannels();
            }
        }

        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            var channels = new List<string>();

            foreach (var item in itemGridView.Items)
            {
                if (item is Channel && itemGridView.SelectedItems.OfType<Channel>().Any(c => c.Id == ((Channel)item).Id))
                {
                    channels.Add(((Channel)item).Id);
                }
            }

            App.SaveChannels(channels);
        }

        private async Task LoadChannels()
        {
            ProgressRingControl.IsActive = true;
            var er = new CachingCoreDataRetrieve();
            var result = await er.GetCoreDataAsync();
            
            var channels = result.Channels.ToList();

            var selectedChannels = channels.Where(c => viewModel.Channels.Any(c2 => c2 == c.Id));

            itemGridView.ItemsSource = channels;
            UpdateLayout();

            foreach (var selectedChannel in selectedChannels)
            {
                itemGridView.SelectedItems.Add(selectedChannel);
            }

            ProgressRingControl.IsActive = false;
        }
    }
}
