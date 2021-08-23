using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VioletCrawler
{
    public class PageFetcher
    {
        private CookieContainer _cookieContainer;

        public PageFetcher() : this(string.Empty)
        {
        }

        public PageFetcher(string cookieFilePath)
        {
            string cookieJson = File.ReadAllText(cookieFilePath);

            _cookieContainer = new CookieContainer();
            foreach (var item in JsonConvert.DeserializeObject<Cookie[]>(cookieJson))
            {
                _cookieContainer.Add(item);
            }
        }

        public async Task<HtmlDocument> LoadPageAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 10000;
            request.Method = "GET";

            request.CookieContainer = _cookieContainer;

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            var stream = response.GetResponseStream();

            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                return doc;
            }
        }

        public void DownloadFile(string url, string basePath)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.CookieContainer = _cookieContainer;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                var fn = response.Headers["Content-Disposition"].Split(new string[] { "=" }, StringSplitOptions.None)[1];
                fn = fn.Substring(1, fn.Length - 2);
                var responseStream = response.GetResponseStream();
                using (var fileStream = File.Create(Path.Combine(basePath, fn)))
                {
                    responseStream.CopyTo(fileStream);
                }
            }
        }
    }
}
