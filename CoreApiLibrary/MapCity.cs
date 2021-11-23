using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreApiLibrary
{
    public class MapCity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public Int32 Id { get; set; }
        [JsonProperty("latitude")]
        public String Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitute { get; set; }
        [JsonProperty("url")]
        public string urlMuseeum { get; set; }
    }
}
