﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Pa_TV.Extensions;
using Pa_TV.Models;
using Pa_TV.Service;
using Pa_TV.ViewModels;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Pa_TV
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public const string DateFormat = "yyyy-MM-ddTHH:mm";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            //IRetrieveCoreData cdr = new CoreDataRetriever();
            //var x = cdr.GetCoreDataAsync().Result;

            //IRetrieveEvents er = new EventRetriever();
            //var x = er.GetEventsTodayAsync().Result;

            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        public static IEnumerable<string> GetChannelsOrDefault(ApplicationData data = null)
        {
            if (data == null)
                data = ApplicationData.Current;

            var channels = data.RoamingSettings.Values["Channels"] as string;

            if (string.IsNullOrWhiteSpace(channels))
                return new List<string> { "10","11","9","12","299","15","21","156" };

            return channels.ToObject<ChannelGroup>().ChannelIds;
        }

        public static void SaveChannels(IEnumerable<string> channels)
        {
            var channelGroup = new ChannelGroup();
            channelGroup.ChannelIds = channels;
            ApplicationData.Current.RoamingSettings.Values["Channels"] = channelGroup.ToJson();
            ApplicationData.Current.SignalDataChanged();
        }

        public static IEnumerable<string> GetFavoritesOrDefault(ApplicationData data = null)
        {
            if (data == null)
                data = ApplicationData.Current;

            var favorites = data.RoamingSettings.Values["Favorites"] as string;

            if (string.IsNullOrWhiteSpace(favorites))
                return new List<string>();

            return favorites.ToObject<FavoriteGroup>().Favorites;
        }

        public static void SaveFavorites(IEnumerable<string> favorites)
        {
            var favoritGroup = new FavoriteGroup();
            favoritGroup.Favorites = favorites;
            ApplicationData.Current.RoamingSettings.Values["Favorites"] = favoritGroup.ToJson();
            ApplicationData.Current.SignalDataChanged();
        }
    }
}
