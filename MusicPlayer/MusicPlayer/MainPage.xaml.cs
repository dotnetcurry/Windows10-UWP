using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BackgroundAudioShared;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MusicPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Get User
            GetUserDetails();
        }

        private async void GetUserDetails()
        {
            try
            {
                string responseText = await GetjsonStream(App.SoundCloudLink + App.SoundCloudAPIUsers + "shoban-kumar" + ".json?client_id=" + App.SoundCloudClientId);
                App.SCUser = JsonConvert.DeserializeObject<SoundCloudUser>(responseText);

                App.SCUserID = App.SCUser.id;

                //Get Likes 
                GetLikes();
            }
            catch (Exception ex)
            {
                MessageDialog showMessgae = new MessageDialog("Something went wrong. Please try again. Error Details : " + ex.Message);
                await showMessgae.ShowAsync();
            }
        }

        private async void GetLikes()
        {

            try
            {

                string responseText = await GetjsonStream(App.SoundCloudLink + App.SoundCloudAPIUsers + App.SCUserID + "/favorites.json?client_id=" + App.SoundCloudClientId);
                App.likes = JsonConvert.DeserializeObject<List<SoundCloudTrack>>(responseText);

                //remove songs which do not have stream url
                App.likes = App.likes.Where(t => t.stream_url != null).ToList<SoundCloudTrack>();

                //add "?client_id=" + App.SoundCloudClientId to stream url
                App.likes = App.likes.Select(t => { t.stream_url += "?client_id=" + App.SoundCloudClientId; return t; }).ToList<SoundCloudTrack>();


                loginProgress.IsActive = false;

                AppShell shell = Window.Current.Content as AppShell;
                shell.AppFrame.Navigate(typeof(NowPlaying));
            }
            catch (Exception ex)
            {
                MessageDialog showMessgae = new MessageDialog("Something went wrong. Please try again. Error Details : " + ex.Message);
                await showMessgae.ShowAsync();
            }

        }

        public async Task<string> GetjsonStream(string url) //Function to read from given url
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            HttpResponseMessage v = new HttpResponseMessage();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
