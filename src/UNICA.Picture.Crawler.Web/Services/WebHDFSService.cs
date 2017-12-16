using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace UNICA.Picture.Crawler.Web.Services
{
    public class WebHDFSService
    {
        private readonly ILogger m_log;
        private readonly IHostingEnvironment m_env;

        public WebHDFSService(ILogger log, IHostingEnvironment env)
        {
            m_log = log;
            m_env = env;
        }

        public async Task WriteToWebHdfs(string json, string urlBase)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var unit = new DateTime(1993, 6, 21);
                    var elapsed = DateTime.Now - unit;
                    var url = $"{urlBase}{elapsed.Milliseconds}.json?op=CREATE&overwrite=true";

                    var location = await client.PutAsync(new Uri(url), new StringContent(json));
                    if (location.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        IEnumerable<string> locations = null;
                        if (location.Headers.TryGetValues("Location", out locations))
                        {
                            var hdfs = locations.FirstOrDefault();
                            m_log.LogInformation($"[WEBHDFS] => New file @ {hdfs}");
                        }
                        else
                        {
                            m_log.LogInformation($"[WEBHDFS] => Unable to find new file location");
                        }
                    }
                    else
                    {
                        Fallback(json);
                        var content = await location.Content.ReadAsStringAsync();
                        m_log.LogError($"[WEBHDFS] => Request result: {location.StatusCode.ToString()} - Content: {content}");
                    }
                }
            }
            catch (Exception ex)
            {
                Fallback(json);
                m_log.LogError($"[WEBHDFS] => {ex.Message}");
            }
        }

        private void Fallback(string json)
        {
            var path = Path.Combine(m_env.ContentRootPath, "Crawls");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, $"{Guid.NewGuid().ToString()}.json"), json);
        }
    }
}
