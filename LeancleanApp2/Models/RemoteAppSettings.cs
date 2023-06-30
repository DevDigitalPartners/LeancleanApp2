using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeancleanApp2.Models
{
    public class RemoteAppSettings
    {
        [JsonProperty("values")]
        public List<KeyValuePair<string, string>> Settings { get; set; }
    }
}
