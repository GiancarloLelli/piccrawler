using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UNICA.Picture.Crawler.Web.Models;

namespace UNICA.Picture.Crawler.Web.Services
{
    public class FacebookGraphService
    {
        public static async Task<string> FetchPostData(string token)
        {
            var profileDataJson = await Get("me?fields=id", token);
            var hubData = JsonConvert.DeserializeObject<HubData>(profileDataJson.ToString());

            var postDataJson = await Get("me/feed?limit=275", token);
            var postData = JsonConvert.DeserializeObject<FacebookPostResponse>(postDataJson.ToString());
            FacebookPostResponse lastPage = null;

            while (postData.PagingInfo != null)
            {
                postDataJson = await Get(postData.PagingInfo.Next, token);
                lastPage = postData;
                postData = JsonConvert.DeserializeObject<FacebookPostResponse>(postDataJson.ToString());
            }

            var firstPost = lastPage.Posts.OrderBy(p => DateTime.Parse(p.CreatedOn)).FirstOrDefault();
            hubData.Result = firstPost;

            return hubData.Result.Id;
        }

        public static async void FetchPhotoAndSendToSpark(string token, string sparkUrl, ILogger log, IHostingEnvironment env)
        {
            var profileDataJson = await Get("me?fields=id", token);
            var hubData = JsonConvert.DeserializeObject<SparkData>(profileDataJson.ToString());

            var postDataJson = await Get("me/photos?fields=images&limit=100", token);
            var postData = JsonConvert.DeserializeObject<FacebookPhotoResponse>(postDataJson.ToString());

            List<string> urls = new List<string>();
            urls.AddRange(postData.Photos.Select(p => p.Image.OrderByDescending(r => r.Width).ThenByDescending(s => s.Height).FirstOrDefault()?.Url));

            while (postData.PagingInfo?.Next != null)
            {
                postDataJson = await Get(postData.PagingInfo.Next, token);
                postData = JsonConvert.DeserializeObject<FacebookPhotoResponse>(postDataJson.ToString());
                urls.AddRange(postData.Photos.Select(p => p.Image.OrderByDescending(r => r.Width).ThenByDescending(s => s.Height).FirstOrDefault()?.Url));
            }

            hubData.Images = urls;

            if (!Debugger.IsAttached)
                await (new WebHDFSService(log, env)).WriteToWebHdfs(JsonConvert.SerializeObject(hubData), sparkUrl);
        }

        private static async Task<string> Get(string resource, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://graph.facebook.com/v2.8/");
                var resp = await client.GetAsync($"{resource}&access_token={token}");
                return await resp.Content.ReadAsStringAsync();
            }
        }
    }
}