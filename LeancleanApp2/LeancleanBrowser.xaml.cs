using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace LeancleanApp2
{
    public partial class LeancleanBrowser : ContentPage
    {
        private string originalUrl;
        public LeancleanBrowser(string url)
        {
            InitializeComponent();
            
            Init(url);
        }

        private void Init(string url)
        {
            //var pass = await Xamarin.Essentials.SecureStorage.GetAsync(Data.Settings.ActiveUser);
            //Helpers.RESTService.EnableAuthentication(Data.Settings.ActiveUser, pass);
            //url = "https://leancleanstaging.azurewebsites.net";
            Browser.Source = url;
            Browser.BackgroundColor = Color.Green;
            
            originalUrl = url;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        //Added for future compatability
        public void OnWebStartNavigating(object sender, WebNavigatingEventArgs e)
        {

            if (e.Url.StartsWith("mailto") || e.Url.StartsWith("tel"))
            {
                Launcher.OpenAsync(new Uri(e.Url));
                e.Cancel = true;
            }


            if (e.Url.EndsWith(".pdf"))
            {
                Launcher.OpenAsync(new Uri(e.Url));
                e.Cancel = true;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return BrowserGoBack();
        }

        public void Handle_SwipedLeft(object sender, Xamarin.Forms.SwipedEventArgs e)
        {
            BrowserGoForward();
        }

        public void Handle_SwipedRight(object sender, Xamarin.Forms.SwipedEventArgs e)
        {
            BrowserGoBack();
        }

        public bool BrowserGoForward()
        {
            if (Browser.CanGoForward)
            {
                Browser.GoForward();
            }
            return true;
        }

        public bool BrowserGoBack()
        {
            if (Browser.CanGoBack)
            {
                Browser.GoBack();
            }
            return true;
        }

        async void NavigateTo(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new QRScanner());
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new QRScanner());
            NavigationPage np = new NavigationPage();
            Device.InvokeOnMainThreadAsync(() =>
            {
                np.PushAsync(new QRScanner());
            });
            
        }
    }
}
