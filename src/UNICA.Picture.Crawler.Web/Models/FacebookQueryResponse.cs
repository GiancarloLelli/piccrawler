using Newtonsoft.Json;
using System.Collections.Generic;

namespace UNICA.Picture.Crawler.Web.Models
{
    [JsonObject]
    public class FacebookPostResponse
    {
        [JsonProperty(PropertyName = "data")]
        public IEnumerable<Post> Posts { get; set; }

        [JsonProperty(PropertyName = "paging")]
        public Paging PagingInfo { get; set; }
    }

    [JsonObject]
    public class FacebookPhotoResponse
    {
        [JsonProperty(PropertyName = "data")]
        public List<PhotoData> Photos { get; set; }

        [JsonProperty(PropertyName = "paging")]
        public Paging PagingInfo { get; set; }
    }

    [JsonObject]
    public class PhotoData
    {
        [JsonProperty(PropertyName = "images")]
        public IEnumerable<FacebookImage> Image { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }

    [JsonObject]
    public class FacebookImage
    {
        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }
    }

    [JsonObject]
    public class Post
    {
        [JsonProperty(PropertyName = "message")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "created_time")]
        public string CreatedOn { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }

    [JsonObject]
    public class Paging
    {
        [JsonProperty(PropertyName = "previous")]
        public string Pre { get; set; }

        [JsonProperty(PropertyName = "next")]
        public string Next { get; set; }
    }

    [JsonObject]
    public class HubData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "first_post")]
        public Post Result { get; set; }
    }

    [JsonObject]
    public class SparkData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "urls")]
        public IEnumerable<string> Images { get; set; }
    }
}