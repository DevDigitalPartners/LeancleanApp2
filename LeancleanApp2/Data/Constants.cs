using System;
using System.Collections.Generic;
using System.Text;

namespace LeancleanApp2.Data
{
    public static class Constants
    {
        //app delegator url
        public static readonly string APP_SETTINGS_SERVER_URL = "https://appdelegator.azurewebsites.net/";
        //aapublic static readonly string APP_SETTINGS_SERVER_URL = "https://leancleanstaging.azurewebsites.net";
        public static readonly string APP_SETTINGS_IDENTIFIER = "dk.incaptiva.leanclean"; //{0} country code - each country needs to be a seperate app for now
        public static readonly string APP_SETTINGS_GET_SETTINGS_ENDPOINT = @"api/values/{0}/{1}"; //{0} app identifier {1} app version
        public static readonly int APP_CURRENT_VERSION = 1;

        public static readonly string REST_CONTENT_TYPE = @"application/json";
        public static readonly Encoding REST_CHARSET_ENCODING = Encoding.UTF8;
        public static readonly string REST_CHARSET_TEXT = @"utf-8";

        public static readonly string BASE_STARTING_POINT = @"/Identity/Account/Login";

        public static readonly string FIREBASE_SERVER_KEY = @"";


        public static readonly string REST_API_USER_HAS_ACCESS = @"/api/Login/{0}"; //{0} user email   
        public static readonly string REST_API_EMAIL_VALIDATION_REQUEST = @"/api/Login/{0}/{1}"; //{0} user email {1} AppKey (password)
        public static readonly string REST_API_EMAIL_IS_VALIDATED = @"/api/Login/{0}/{1}"; //{0} user email {1} AppKey (password)
        public static readonly string REST_AUTH_USERNAME_KEY = @"email";
        public static readonly string REST_AUTH_PASSWORD_KEY = @"password";
        public static readonly Helpers.RESTService.AUTHSCHEME REST_AUTH_SCHEME = Helpers.RESTService.AUTHSCHEME.BEARER;
        public static readonly TimeSpan STANDARD_EMAIL_VALIDATION_CHECK_TIME = new TimeSpan(0, 0, 0, 20, 0);
        public static readonly TimeSpan STANDARD_EMAIL_VALIDATION_START_TIME = new TimeSpan(0, 0, 0, 30, 0);
        public static readonly int STANDARD_EMAIL_VALIDATION_MAX_RETRIES = 30;

        //these should be created on the site and be set to email validated on the server son apple can use this email when testing - the app skips certain parts of the flow when this is detected...
        public static readonly string TEST_USER_EMAIL = "default@user.dk";
        public static readonly string TEST_USER_APPKEY = "ae493828-656d-4b0e-a771-9bacecc67ea9";


#if DEBUG
        public static readonly string SENTRY_DSN = @"https://cbcb4410c12647b0bbd473ee30c09045@sentry.io/2205819";
#else
        public static readonly string SENTRY_DSN = @"https://cbcb4410c12647b0bbd473ee30c09045@sentry.io/2205819";
#endif


        public static readonly string REST_AUTH_ENDPOINT = @"/api/Token";



    }
}
