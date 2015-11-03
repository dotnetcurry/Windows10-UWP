using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BackgroundAudioShared;
using BackgroundAudioShared.Messages;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Likes : Page
    {
        public Likes()
        {
            this.InitializeComponent();
            this.Loaded += Likes_Loaded;
        }

        private void Likes_Loaded(object sender, RoutedEventArgs e)
        {
            grdLikes.ItemsSource = App.likes;
        }

        private void grdLikes_ItemClick(object sender, ItemClickEventArgs e)
        {
            var song = e.ClickedItem as SoundCloudTrack;
            MessageService.SendMessageToBackground(new TrackChangedMessage(new Uri(song.stream_url)));

        }

    }
}
