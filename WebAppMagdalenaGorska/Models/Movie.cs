using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppOpenIDConnectDotNet.Models;

namespace WebApp_OpenIDConnect_DotNet.Models
{
    public class Movie
{
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("director")]
        public string Director { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
