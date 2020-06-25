﻿using MediaManager;
using MediaManager.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Player;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFAudioPlayer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaListPage : ContentPage
    {
        public const string PlayCircleOutline = "\U000f040d";
        public const string PauseCircleOutline = "\U000f03e6";

        public MediaListPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(() =>
            {
                if (CrossMediaManager.Current.Queue.Count > 0)
                    SetupInitData();
            });
        }

        private async void ButtonClose_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void ButtonPlayPause_Clicked(object sender, EventArgs e)
        {
            if (CrossMediaManager.Current.IsPlaying())
            {
                await CrossMediaManager.Current.Pause();
                ButtonPlayPause.Text = PlayCircleOutline;
            }
            else
            {
                await CrossMediaManager.Current.Play();
                ButtonPlayPause.Text = PauseCircleOutline;
            }
        }

        private async void ButtonPlaySong_Clicked(object sender, EventArgs e)
        {
            var selectedAudioItem = (((Button) sender).BindingContext as AudioItem);
            var result = await CrossMediaManager.Current.PlayQueueItem(CrossMediaManager.Current.Queue[selectedAudioItem.Index - 1]);
            SetupCurrentItemDetails();

            var isPlaying = CrossMediaManager.Current.IsPlaying();
            ButtonPlayPause.Text = PauseCircleOutline;
        }

        private void SetupPlaylist(bool isListRefresh)
        {
            List<IMediaItem> queueList = CrossMediaManager.Current.Queue.ToList();

            ObservableCollection<AudioItem> list = new ObservableCollection<AudioItem>();
            for (int i = 0; i < queueList.Count; i++)
            {
                IMediaItem item = queueList[i];

                string title = "-"; string artist = "-";

                if (!string.IsNullOrEmpty(item.DisplayTitle))
                    title = $"{item.DisplayTitle.ToUpper()}";

                if (!string.IsNullOrEmpty(item.Artist))
                    artist = $"{item.Artist.ToUpper()}";

                list.Add(new AudioItem()
                {
                    Title = title,
                    Artist = artist,
                    Index = i + 1
                });
            }

            CollectionView.ItemsSource = list;

            if (isListRefresh)
                CollectionView.ScrollTo(list.Last());
        }

        private void SetupCurrentItemDetails()
        {
            IMediaItem currentAudioItem = CrossMediaManager.Current.Queue.Current;

            // Media item details
            var displayDetails = string.Empty;
            if (!string.IsNullOrEmpty(currentAudioItem.DisplayTitle))
                displayDetails = currentAudioItem.DisplayTitle;

            if (!string.IsNullOrEmpty(currentAudioItem.Artist))
                displayDetails = $"{displayDetails} - {currentAudioItem.Artist}";

            LabelCurrentTrackTitle.Text = displayDetails.ToUpper();

            LabelCurrentTrackIndex.Text = $"CURRENT TRACK: {CrossMediaManager.Current.Queue.CurrentIndex + 1}/{CrossMediaManager.Current.Queue.Count}";
        }

        private void SetupInitData()
        {
            SetupPlaylist(false);
            SetupCurrentItemDetails();

            var isPlaying = CrossMediaManager.Current.IsPlaying();
            ButtonPlayPause.Text = isPlaying ? PauseCircleOutline : PlayCircleOutline;
        }
    }

    public class AudioItem 
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public int Index { get; set; }
    }
}