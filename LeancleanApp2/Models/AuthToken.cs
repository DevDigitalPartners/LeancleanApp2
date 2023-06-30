using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeancleanApp2.Models
{
    public class AuthToken
    {
        [JsonProperty("Token")]
        public string Token { get; set; }
    }
}
