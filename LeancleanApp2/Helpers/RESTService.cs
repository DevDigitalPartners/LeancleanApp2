using Sentry;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LeancleanApp2.Helpers
{
    public static class RESTService
    {
        private static HttpClient client;
        private static string userToken;
        private static bool useAuthentication;
        private static string authUserName;
        private static string authUserNameKey;
        private static string authPassword;
        private static string authPasswordKey;
        private static AUTHSCHEME authScheme;
        private static string authTokenProviderEndpoint;
        private static ITokenWrapper authTokenWrapper;
        private static SentryClient sen;

        public enum AUTHSCHEME
        {
            NONE = 0,
            UNKNOWN = 1,
            BASIC = 2,
            BEARER = 4
        }

        //public static bool UseSentry { get; set; }

        static RESTService()
        {
            if (client == null)
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(Data.Settings.LiveUrl);
                client.MaxResponseContentBufferSize = 256000; //~2MB
                client.Timeout = new TimeSpan(0, 0, 2, 0, 0);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(Data.Constants.REST_CONTENT_TYPE));
                client.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue(Data.Constants.REST_CHARSET_TEXT));

            }
        }

        public static void EnableAuthentication(string username, string password)
        {
            EnableAuthentication(username, password, Data.Constants.REST_AUTH_USERNAME_KEY, Data.Constants.REST_AUTH_PASSWORD_KEY, Data.Settings.LiveUrl + Data.Constants.REST_AUTH_ENDPOINT, Data.Constants.REST_AUTH_SCHEME, new TokenWrapper());
        }

        public static void UpdateAuthenticationValues(string username, string password)
        {
            UpdateAuthenticationValues(username, password, Data.Constants.REST_AUTH_USERNAME_KEY, Data.Constants.REST_AUTH_PASSWORD_KEY, Data.Settings.LiveUrl + Data.Constants.REST_AUTH_ENDPOINT, Data.Constants.REST_AUTH_SCHEME, new TokenWrapper());
        }

        //public static void EnableAuthentication(string username, string password, string usernamekey, string passwordkey, string tokenurl, string scheme, ITokenWrapper wrapper)
        public static void EnableAuthentication(string username, string password, string usernamekey, string passwordkey, string tokenurl, AUTHSCHEME scheme, ITokenWrapper wrapper)
        {
            authUserName = username;
            authPassword = password;
            authUserNameKey = usernamekey;
            authPasswordKey = passwordkey;
            authTokenProviderEndpoint = tokenurl;
            authScheme = scheme;
            authTokenWrapper = wrapper;
            useAuthentication = true;
        }

        //public static void UpdateAuthenticationValues(string username, string password, string usernamekey, string passwordkey, string tokenurl, string scheme, ITokenWrapper wrapper)
        public static void UpdateAuthenticationValues(string username, string password, string usernamekey, string passwordkey, string tokenurl, AUTHSCHEME scheme, ITokenWrapper wrapper)
        {
            authUserName = username;
            authPassword = password;
            authUserNameKey = usernamekey;
            authPasswordKey = passwordkey;
            authTokenProviderEndpoint = tokenurl;
            authTokenWrapper = wrapper;
            authScheme = scheme;
        }

        public static void DisableAuthentication()
        {
            client.DefaultRequestHeaders.Authorization = null;
            useAuthentication = false;
        }

        private static async Task<bool> UpdateAutherization()
        {
            if (useAuthentication)
            {
                switch (authScheme)
                {
                    case AUTHSCHEME.BASIC:
                        //should this be ascii encoding??
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", authUserName, authPassword))));
                        break;

                    case AUTHSCHEME.BEARER:

                        userToken = await GetBearerToken(authUserName, authPassword, authUserNameKey, authPasswordKey, authTokenProviderEndpoint, authTokenWrapper);
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
                        break;
                }
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authScheme) 
            }

            return true;
        }

        public static async Task<string> GetAuthHeader(bool refresh)
        {
            //should it check if token/auth is still valid before returning it?

            if (client.DefaultRequestHeaders.Authorization == null || refresh)
            {
                //this seems to never get a response or an exception when asking the server for a token...is there maybe a problem if the user already have got a token??
                await UpdateAutherization();

                //sync version seems to work...
                //UpdateAutherizationSync();
            }

            return client.DefaultRequestHeaders.Authorization.Scheme + " " + client.DefaultRequestHeaders.Authorization.Parameter;
        }

        public static async Task<string> GetBearerToken(string username, string password, string usernamekey, string passwordkey, string tokenurl, ITokenWrapper wrapper)
        {
            try
            {
                string token = "";

                var body = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>(usernamekey, username),
                    new KeyValuePair<string, string>(passwordkey, password)
                });

                //for some reason await does not work, but calling result does?...
                var response = client.PostAsync(tokenurl, body).Result; //fetch a user token
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Problem fetching token for " + username);
                }

                //if no wrapper assigned, assume that the token is simply the returned string
                if (wrapper == null)
                {
                    token = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    token = await wrapper.GetToken(response);
                }

                return token;
            }
            catch (Exception e)
            {
                AppSentry.CaptureException(e);
                return String.Empty;
            }
        }

        public static async Task<string> GetAsync(string endPoint)
        {
            var response = await client.GetAsync(endPoint);

            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        if (useAuthentication)
                        {
                            await UpdateAutherization();
                            response = await client.GetAsync(endPoint);
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            //throw new Exception("Could not authenticate user for " + endPoint);
                            throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                        }
                        break;

                    default:
                        throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                }
                //throw new Exception();
            }
            return await response.Content.ReadAsStringAsync();
        }

        /*
        public static string GetSync(string endPoint)
        {
            var response = client.GetAsync(endPoint).Result;

            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        if (useAuthentication)
                        {
                            bool t = UpdateAutherizationSync();
                            response = client.GetAsync(endPoint).Result;
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                        }
                        break;

                    default:
                        throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                }
                //throw new Exception();
            }
            return response.Content.ReadAsStringAsync().Result;
        }
        */

        public static async Task<string> GetAsync(string endPoint, string id)
        {
            //Uri url = new Uri(Data.Constants.REST_BASE_URL + endPoint + "/" + id + "/");
            //var response = await client.GetAsync(url);
            var response = await client.GetAsync(endPoint + "/" + id);
            //var response = await client.GetAsync("https://patriotiskselskab.azurewebsites.net/api/Polls/");
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        if (useAuthentication)
                        {
                            await UpdateAutherization();
                            response = await client.GetAsync(endPoint + "/" + id);
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint + "/" + id);
                        }
                        break;

                    default:
                        throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint + "/" + id);
                }
            }
            return await response.Content.ReadAsStringAsync();
        }

        //TODO:should be rewritten so duplicate checks can be avoided
        public static async Task<string> GetAsync(string endPoint, string id, bool throwErrorOnNotFound)
        {

            var response = await client.GetAsync(endPoint + "/" + id);
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        if (useAuthentication)
                        {
                            await UpdateAutherization();
                            response = await client.GetAsync(endPoint + "/" + id);
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            {
                                if (throwErrorOnNotFound)
                                {
                                    throw new Exception("404 - Not found");
                                }
                                return String.Empty;
                            }
                            throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint + "/" + id);
                        }
                        break;
                    case System.Net.HttpStatusCode.NotFound:
                        if (throwErrorOnNotFound)
                        {
                            throw new Exception("404 - Not found");
                        }
                        return String.Empty;
                        break;
                    default:
                        //return String.Empty; //TEMPORARY !!!!
                        throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint + "/" + id);
                }
            }
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> PostAsync(string endPoint, HttpContent content)
        {
            var response = await client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        if (useAuthentication)
                        {
                            await UpdateAutherization();
                            response = await client.PostAsync(endPoint, content);
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                        }
                        break;

                    default:
                        throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                }
            }
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> PostAsync(string endPoint, HttpContent content, string id)
        {
            var response = await client.PostAsync(endPoint + "//" + id, content);
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        if (useAuthentication)
                        {
                            await UpdateAutherization();
                            response = await client.PostAsync(endPoint + "//" + id, content);
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint + "//" + id);
                        }
                        break;

                    default:
                        throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint + "//" + id);
                }
            }
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> PutAsync(string endPoint, HttpContent content)
        {
            var response = await client.PutAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        if (useAuthentication)
                        {
                            await UpdateAutherization();
                            response = await client.PutAsync(endPoint, content);
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                        }
                        break;

                    default:
                        throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                }
            }
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> PutAsync(string endPoint, HttpContent content, string id)
        {
            var response = await client.PutAsync(endPoint + "//" + id, content);
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        if (useAuthentication)
                        {
                            await UpdateAutherization();
                            response = await client.PutAsync(endPoint + "//" + id, content);
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint + "//" + id);
                        }
                        break;

                    default:
                        throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint + "//" + id);
                }
            }
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task DeleteAsync(string endPoint)
        {
            var response = await client.DeleteAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        if (useAuthentication)
                        {
                            await UpdateAutherization();
                            response = await client.DeleteAsync(endPoint);
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                        }
                        break;

                    default:
                        throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                }
            }

        }

        public static async Task DeleteAsync(string endPoint, string id)
        {
            var response = await client.DeleteAsync(endPoint + "//" + id);
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        if (useAuthentication)
                        {
                            await UpdateAutherization();
                            response = await client.DeleteAsync(endPoint + "//" + id);
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint + "//" + id);
                        }
                        break;

                    default:
                        throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint + "//" + id);
                }
            }
        }

        //should this be able to make a login and retry or??
        //How to handle an unknown type of httpclient call if this should handle retries?
        //This should also throw more specific httpexceptions based on response code, if it can't handle them internally
        private async static Task<HttpResponseMessage> HandleResponses(HttpResponseMessage response, string endPoint)
        {
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        if (useAuthentication)
                        {
                            await UpdateAutherization();
                            //   response = await client.DeleteAsync(endPoint + "//" + id);
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                        }
                        break;

                    default:
                        throw new Exception("Response " + response.StatusCode + " " + response.ReasonPhrase + " received for " + endPoint);
                }
            }

            return response;
        }

    }
}
