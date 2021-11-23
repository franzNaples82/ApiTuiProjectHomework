using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreApiLibrary
{
    public class MapForecastCity
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("wheather date")]
        public string date { get; set; }

        [JsonProperty("wheather condition")]
        public string condition { get; set; }
    }
}
