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
using MusicPlayer.Controls;
using Windows.UI.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Automation;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicPlayer
{

    public sealed partial class AppShell : Page
    {
        // Declare the top level nav items
        private List<NavMenuItem> navlist = new List<NavMenuItem>(
            new[]
            {
                new NavMenuItem()
                {
                    Symbol = Symbol.Play,
                    Label ="Now Playing",
                    DestPage = typeof(NowPlaying)
                },
                new NavMenuItem()
                {
                    Symbol = Symbol.Emoji2,
                    Label = "Likes",
                    DestPage = typeof(Likes)
                },
               new NavMenuItem()
                {
                    Symbol = Symbol.Contact,
                    Label = "Me",
                    DestPage = typeof(Me)
                }

            });

        public static AppShell Current = null;

        /// <summary>
        /// Initializes a new instance of the AppShell, sets the static 'Current' reference,
        /// adds callbacks for Back requests and changes in the SplitView's DisplayMode, and
        /// provide the nav menu list with the data to display.
        /// </summary>
        public AppShell()
        {
            this.InitializeComponent();

            this.Loaded += (sender, args) =>
            {
                Current = this;

            };
            NavMenuList.ItemsSource = navlist;
        }


        public Frame AppFrame { get { return this.frame; } }


        /// <summary>
        /// Navigate to the Page for the selected <paramref name="listViewItem"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="listViewItem"></param>
        private void NavMenuList_ItemInvoked(object sender, ListViewItem listViewItem)
        {
            var item = (NavMenuItem)((NavMenuListView)sender).ItemFromContainer(listViewItem);

            if (item != null)
            {
                if (item.DestPage != null &&
                    item.DestPage != this.AppFrame.CurrentSourcePageType)
                {
                    this.AppFrame.Navigate(item.DestPage, item.Arguments);
                }
            }
        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((Page)sender).Focus(FocusState.Programmatic);
            ((Page)sender).Loaded -= Page_Loaded;
        }

    }
}
