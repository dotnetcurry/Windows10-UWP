using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using BackgroundAudioShared.Messages;
using BackgroundAudioTask;
using BackgroundAudioShared;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Media.Playback;
using Windows.UI.Core;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlaying : Page
    {
        private bool isMyBackgroundTaskRunning = false;
        AppShell shell = Window.Current.Content as AppShell;
        private AutoResetEvent backgroundAudioTaskStarted;

        /// <summary>
        /// Gets the information about background task is running or not by reading the setting saved by background task.
        /// This is used to determine when to start the task and also when to avoid sending messages.
        /// </summary>
        private bool IsMyBackgroundTaskRunning
        {
            get
            {
                if (isMyBackgroundTaskRunning)
                    return true;

                string value = ApplicationSettingsHelper.ReadResetSettingsValue(ApplicationSettingsConstants.BackgroundTaskState) as string;
                if (value == null)
                {
                    return false;
                }
                else
                {
                    try
                    {
                        isMyBackgroundTaskRunning = EnumHelper.Parse<BackgroundTaskState>(value) == BackgroundTaskState.Running;
                    }
                    catch (ArgumentException)
                    {
                        isMyBackgroundTaskRunning = false;
                    }
                    return isMyBackgroundTaskRunning;
                }
            }
        }


        public NowPlaying()
        {
            this.InitializeComponent();

            this.Loaded += NowPlaying_Loaded;
        }

        private void NowPlaying_Loaded(object sender, RoutedEventArgs e)
        {
            backgroundAudioTaskStarted = new AutoResetEvent(false);

            if (!IsMyBackgroundTaskRunning)
            {
                StartBackgroundAudioTask();
            }
            else
            {
                //Start playback if Paused.
                if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState)
                {
                    BackgroundMediaPlayer.Current.Play();
                }
                else if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState)
                {
                    StartBackgroundAudioTask();
                }
            }

        }

        private void StartBackgroundAudioTask()
        {
            AddMediaPlayerEventHandlers();
            var startResult = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool result = backgroundAudioTaskStarted.WaitOne(10000);
                //Send message to initiate playback
                if (result == true)
                {
                    MessageService.SendMessageToBackground(new UpdatePlaylistMessage(App.likes));
                    MessageService.SendMessageToBackground(new StartPlaybackMessage());

                }
                else
                {
                    throw new Exception("Background Audio Task didn't start in expected time");
                }
            });
        }

        private void AddMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Update UI based on Track Id stored.
            var trackId = GetCurrentTrackId();
            if (trackId != null)
            {
                var song = App.likes.Where(t => t.stream_url == trackId.ToString()).FirstOrDefault();
                LoadTrack(song);
            }

        }



        async void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            TrackChangedMessage trackChangedMessage;
            if (MessageService.TryParseMessage(e.Data, out trackChangedMessage))
            {
                // When foreground app is active change track based on background message
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var songIndex = GetSongIndexById(trackChangedMessage.TrackId);
                    if (songIndex >= 0)
                    {
                        var song = App.likes[songIndex];
                        LoadTrack(song); //Update UI
                    }
                });
                return;
            }

            BackgroundAudioTaskStartedMessage backgroundAudioTaskStartedMessage;
            if (MessageService.TryParseMessage(e.Data, out backgroundAudioTaskStartedMessage))
            {
                backgroundAudioTaskStarted.Set();
                return;
            }
        }

        public int GetSongIndexById(Uri id)
        {
            return App.likes.FindIndex(s => new Uri(s.stream_url) == id);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (isMyBackgroundTaskRunning)
            {
                ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.BackgroundTaskState, BackgroundTaskState.Running.ToString());
            }

            base.OnNavigatedFrom(e);
        }

        private Uri GetCurrentTrackId()
        {
            object value = ApplicationSettingsHelper.ReadResetSettingsValue(ApplicationSettingsConstants.TrackId);
            if (value != null)
                return new Uri((String)value);
            else
                return null;
        }


        private async void LoadTrack(SoundCloudTrack currentTrack)
        {
            try
            {
                //Change album art
                string albumartImage = Convert.ToString(currentTrack.artwork_url);
                if (string.IsNullOrWhiteSpace(albumartImage))
                {
                    albumartImage = @"ms-appx:///Assets/Albumart.png";

                }
                else
                {
                    albumartImage = albumartImage.Replace("-large", "-t500x500");
                }

                albumrtImage.ImageSource = new BitmapImage(new Uri(albumartImage));

                //Change Title and User name
                txtSongTitle.Text = currentTrack.title;
                txtAlbumTitle.Text = Convert.ToString(currentTrack.user.username);

            }
            catch (Exception ex)
            {
                MessageDialog showMessgae = new MessageDialog("Something went wrong. Please try again. Error Details : " + ex.Message);
                await showMessgae.ShowAsync();
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (IsMyBackgroundTaskRunning)
            {
                if (MediaPlayerState.Playing == BackgroundMediaPlayer.Current.CurrentState)
                {
                    BackgroundMediaPlayer.Current.Pause();
                }
                else if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState)
                {
                    BackgroundMediaPlayer.Current.Play();
                }
                else if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState)
                {
                    StartBackgroundAudioTask();
                }
            }
            else
            {
                StartBackgroundAudioTask();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            //Send message to background task
            MessageService.SendMessageToBackground(new SkipNextMessage());

        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            //Send message to background task
            MessageService.SendMessageToBackground(new SkipPreviousMessage());
        }

    }
}
