using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace LeancleanApp2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRScanner : ContentPage
    {
        public QRScanner()
        {
            InitializeComponent();
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            scanner.UseCustomOverlay = true;

        }

        private void ZXingScannerView_OnScanResult(Result result)
        {
            
            Device.BeginInvokeOnMainThread(() =>
            {
                //Gør noget her med scannings resultatet.
                string resultText = result.ToString();
                
                try
                {
                    ResultLabel.Text = resultText;
                    Navigation.RemovePage(this);
                    Navigation.PushAsync(new LeancleanBrowser("https://leancleanstaging.azurewebsites.net/Worker/Joblist/true/" + resultText + ""));
                    
                }
                catch(Exception ex)
                {
                    ResultLabel.Text = resultText;
                }
                
                //ScanResultTest.Text = result.Text + " (type: " + result.BarcodeFormat.ToString() + ")";
            });
        }

    }
}