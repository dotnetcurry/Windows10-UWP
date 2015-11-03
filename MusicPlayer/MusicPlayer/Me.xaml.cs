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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Me : Page
    {
        public Me()
        {
            this.InitializeComponent();
            this.Loaded += Me_Loaded;
        }

        private void Me_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.SCUser != null)
            {
                txtFirstname.Text = Convert.ToString(App.SCUser.first_name);
                txtlastname.Text = Convert.ToString(App.SCUser.last_name);
                txtWebsite.Text = Convert.ToString(App.SCUser.website);
                txtCity.Text = Convert.ToString(App.SCUser.city);
                txtCountry.Text = Convert.ToString(App.SCUser.country);
                txtFollowers.Text = Convert.ToString(App.SCUser.followers_count);
                txtFollowing.Text = Convert.ToString(App.SCUser.followings_count);
                profilePhoto.ImageSource = new BitmapImage(new Uri(App.SCUser.avatar_url));

            }
        }
    }
}
