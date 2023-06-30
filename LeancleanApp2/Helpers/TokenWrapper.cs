using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LeancleanApp2.Helpers
{
    public class TokenWrapper : ITokenWrapper
    {

        public async Task<string> GetToken(HttpResponseMessage response)
        {
            Models.AuthToken token;
            try
            {
                var result = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<Models.AuthToken>(result);
                if (token == null)
                {
                    return String.Empty;
                }
            }
            catch (Exception e)
            {
                AppSentry.CaptureException(e);
                return String.Empty;
            }

            return token.Token;
        }
    }
}
