using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using UNICA.Picture.Crawler.Web.Models;
using UNICA.Picture.Crawler.Web.Services;

namespace UNICA.Picture.Crawler.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string m_appid;
        private readonly string m_secret;
        private readonly string m_callback;
        private readonly string m_sparkurl;
        private readonly ILogger m_log;
        private readonly IHostingEnvironment m_env;

        public HomeController(IOptions<ConfigurationWrapper> config, ILogger<HomeController> log, IHostingEnvironment env)
        {
            m_callback = Debugger.IsAttached ? config.Value.DevelopmentCallback : config.Value.ProductionCallback;
            m_appid = config.Value.ApplicationId;
            m_secret = config.Value.ApplicationSecret;
            m_sparkurl = config.Value.SparkURL;
            m_log = log;
            m_env = env;
        }

        public IActionResult Index()
            => View();

        public IActionResult Login()
            => Redirect($"https://www.facebook.com/v2.8/dialog/oauth?client_id={m_appid}&redirect_uri={m_callback}&response_type=code&scope=user_posts,user_photos");

        public async Task<IActionResult> Post(string code)
        {
            FacebookAuthenticationResponse facebookAuthData = null;

            using (var client = new HttpClient())
            {
                var url = "https://graph.facebook.com/v2.8/oauth/access_token?";
                url = string.Concat(url, $"client_id={m_appid}&", $"redirect_uri={m_callback}&", $"client_secret={m_secret}&", $"code={code}");
                var response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();
                var facebookJson = await response.Content.ReadAsStringAsync();
                facebookAuthData = JsonConvert.DeserializeObject<FacebookAuthenticationResponse>(facebookJson);
            }

            var data = await FacebookGraphService.FetchPostData(facebookAuthData.AccessToken);
            Task.Run(async () => { FacebookGraphService.FetchPhotoAndSendToSpark(facebookAuthData.AccessToken, m_sparkurl, m_log, m_env); });

            ViewData.Add("Post", data);
            return View("Index");
        }
    }
}
