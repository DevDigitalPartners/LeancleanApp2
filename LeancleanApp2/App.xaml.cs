using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LeancleanApp2
{
    public partial class App : Application
    {
        public string LiveUrl { get; set; }
        public App()
        {
            InitializeComponent();
             
            LiveUrl = "";
            UpdateLiveUrl().ConfigureAwait(false);
            MainPage = new NavigationPage(new LeancleanBrowser(Data.Settings.LiveUrl))
            {
                BarBackgroundColor = Color.FromHex("#83C934"),
                BarTextColor = Color.White
            };
            //MainPage = new EnterEmail();
            //switch (Data.Settings.LoginStatus)
            //{

            //    case Data.Enums.LoginFlowStatus.EmailIsValidated:
            //        //MainPage = new InplanningBrowser(Data.Settings.LiveUrl + Data.Constants.BASE_STARTING_POINT);
            //        MainPage = new LeancleanBrowser(Data.Settings.LiveUrl + Data.Constants.BASE_STARTING_POINT + "?deviceId=" + Data.Settings.DeviceID);
            //        break;

            //    case Data.Enums.LoginFlowStatus.InputEmail:
            //        MainPage = new EnterEmail();
            //        MainPage = new LeancleanBrowser("http://www.leancleanstaging.azurewebsites.net");
            //        break;

            //    case Data.Enums.LoginFlowStatus.RequestEmailValidation:
            //        //if we end up here something probably went wrong with the validation so we are giving the user a new chance to enter their email
            //        MainPage = new EnterEmail();
            //        MainPage = new LeancleanBrowser(Data.Settings.LiveUrl + Data.Constants.BASE_STARTING_POINT);

            //        break;

            //    case Data.Enums.LoginFlowStatus.WaitingForEmailValidation:
            //        //MainPage = new WaitingForValidation();
            //        break;

            //}
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        private async Task UpdateLiveUrl()
        {
            LiveUrl = await GetLiveUrl();

            if (!String.IsNullOrWhiteSpace(LiveUrl))
            {
                Data.Settings.LiveUrl = LiveUrl;
                //TempurAppV2.App.Current.MainPage = new TempurBrowser(browserUrl + "?deviceId=" + Data.Settings.DeviceID);
            }
            else
            {
                //LiveUrl = "https://leancleanstaging.azurewebsites.net";
                Data.Settings.LiveUrl = LiveUrl;
            }
        }

        private async Task<string> GetLiveUrl()
        {
            try
            {
                string keyValue = "LIVE_SITE_URL";
                string url = "";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Data.Constants.APP_SETTINGS_SERVER_URL);
                    client.MaxResponseContentBufferSize = 256000; //~2MB
                    client.Timeout = new TimeSpan(0, 0, 2, 0, 0);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(Data.Constants.REST_CONTENT_TYPE));
                    client.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue(Data.Constants.REST_CHARSET_TEXT));

                    string remoteSettingsUrl = Data.Constants.APP_SETTINGS_SERVER_URL + String.Format(Data.Constants.APP_SETTINGS_GET_SETTINGS_ENDPOINT, Data.Constants.APP_SETTINGS_IDENTIFIER, Data.Constants.APP_CURRENT_VERSION);

                    var response = await client.GetAsync(remoteSettingsUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            Models.RemoteAppSettings remote = JsonConvert.DeserializeObject<Models.RemoteAppSettings>(content);
                            Dictionary<string, string> settings = remote.Settings.ToDictionary(x => x.Key, x => x.Value);
#if DEBUG
                            keyValue = "TEST_SITE_URL";
#endif
                            
                            if (!settings.TryGetValue(keyValue, out url))
                             {
                                //Set a default value if entry not found
                                url = "";
                            }

                        }

                    }
                }

                return url;
            }
            catch (Exception e)
            {
                //handle problems with getting settings from server
                return "";
            }

        }
    }
}
