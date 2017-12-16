using Newtonsoft.Json;
using System;

namespace UNICA.Picture.Crawler.Web.Models
{
    [JsonObject]
    public class FacebookAuthenticationResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public string ExpiresSeconds { get; set; }

        public string Name
        {
            get
            {
                return DateTime.Now.Ticks.ToString();
            }
        }
    }
}