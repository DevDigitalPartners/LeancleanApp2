using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;
using ZXing;

namespace LeancleanApp2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnterEmail : ContentPage
    {
        public bool StartScanning { get; set; } = false;
        public EnterEmail()
        {
            InitializeComponent();
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            //NextButton.IsEnabled = false;

            //validate and return if not ok

            //Go to the request validation page
           // Data.Settings.ActiveUser = Email.Text;

           // Data.Settings.LoginStatus = Data.Enums.LoginFlowStatus.RequestEmailValidation;

            //call email validation logic
            await StartEmailValidations();

            

            //App.Current.MainPage = new RequestValidation();

        }

        private async void Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex(@"^((?!\.)[\w\-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$");
            
          //  NextButton.IsEnabled = regex.IsMatch(e.NewTextValue);           
        }

        private async Task StartEmailValidations()
        {

            //Data.Settings.AppKey = Helpers.AppLogin.GenerateAppKey();

            //bool success = await Helpers.AppLogin.RequestEmailValidation(Data.Settings.ActiveUser, Data.Settings.AppKey);

            //if (success)
            //{
            //    Data.Settings.LoginStatus = Data.Enums.LoginFlowStatus.WaitingForEmailValidation;
            //    App.Current.MainPage = new WaitingForValidation();
            //}
            //else
            //{
            //    // TODO: handle this gracefully..
            //    //maybe show - we couldn't find your email...or something like it...please make sure your company has added it to inPlanning

            //    NextButton.IsEnabled = true;
            //}
        }

        private void ScanButton_Clicked(object sender, EventArgs e)
        {
        }

    }
}