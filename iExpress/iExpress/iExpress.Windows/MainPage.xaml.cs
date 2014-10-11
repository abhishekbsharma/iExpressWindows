﻿
using Parse;
using iExpress.Common;
using System;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
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

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
//Testing push
namespace iExpress
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private Boolean entered;
        private Boolean exited;
        private int counter;
        private int internal_counter = 36;
        private int running_counter;
        private String UserName;


        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion




        private void mouseEntered(object sender, PointerRoutedEventArgs e)
        {
            
            Debug.WriteLine(sender.GetHashCode() + "Detected the entering of the button");
            entered = true;
            exited = false;
            //counter = 6;
            //Abhi - Testing if CountDown Testing Works 
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("CountDown"))
            {
                counter = (int)ApplicationData.Current.RoamingSettings.Values["CountDown"];
                counter++;
            }
            else
            {
                counter = 6;
            }

            running_counter = 0;
        }



        private void mouseExited(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine(sender.GetHashCode() + "Detected the exiting of the button");
            exited = true;
            entered = false;

            (sender as Button).Background = null;
        }

        private void mousedMoved(object sender, PointerRoutedEventArgs e)
        {
            if (entered == true && exited == false)
            {
                running_counter++;
                if (running_counter == internal_counter)
                {
                    running_counter = 0;
                    counter--;
                    String location = "ms-appx:///Assets/" + counter + ".png";
                    (sender as Button).Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(location)) };


                }

                if (counter == 1)
                {

                    if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("UserName"))
                        UserName = (String)ApplicationData.Current.RoamingSettings.Values["UserName"];
                    else
                        UserName = "Patient";

                    Debug.WriteLine("Trigger execution!!!!!!!!");
                    (sender as Button).Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Sent.png")) };
                    Button but = (sender as Button);
                    String message = UserName +":"+but.Content.ToString();

                    ParsePush push = new ParsePush();
                    push.Channels = new List<String> { "global" };
                    IDictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("sound", ".");
                    dic.Add("alert", message);
                    push.Data = dic;                      
                    push.SendAsync();


                    ParseObject internal_tweets = new ParseObject("TweetsInternal");
                    internal_tweets["content"] = message;
                    internal_tweets["sender"] = UserName;
                    internal_tweets.SaveAsync();



                    entered = false;
                    exited = true;

                }
            }

        }

        

    }

}
