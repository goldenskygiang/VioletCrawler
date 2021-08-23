using HtmlAgilityPack;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace VioletCrawler
{
    class Program
    {
        const string BASE_URL = "https://baigiang.violet.vn/";

        static PageFetcher htmlWeb = new PageFetcher("cookie.json");

        static async Task Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.Unicode;
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            string url = GetVioletUrl();
            string authorName = GetFavoredAuthorName();
            Console.WriteLine(authorName);
            IEnumerable<Uri> urls = await GetSubUrls(url);

            foreach (var item in urls)
            {
                IEnumerable<Uri> pps = await GetPowerPointList(item.AbsoluteUri);
                string chosenPP = await ChooseFavoredAuthorOrMaxDownloadPP(pps, authorName);
                await SavePP(chosenPP, Environment.CurrentDirectory);
            }
        }

        static string GetVioletUrl()
        {
            Console.Write("Enter Violet subdirectory URL: ");
            string url = Console.ReadLine();

            while (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.Write("Invalid URL! Enter Violet subdirectory URL: ");
                url = Console.ReadLine();
            }

            return url;
        }

        static string GetFavoredAuthorName()
        {
            Console.Write("Enter proper favored Author Name: ");
            string name = Console.ReadLine();
            return name;
        }

        static async Task<IEnumerable<Uri>> GetSubUrls(string mainUrl)
        {
            HtmlDocument doc = await htmlWeb.LoadPageAsync(mainUrl);
            var mainFrame = doc.DocumentNode.Descendants("div").First(d => d.HasClass("frame-main"));
            var content = mainFrame.Descendants("div").First(d => d.HasClass("content"));
            var lessons = content.Elements("b");

            List<Uri> urls = new List<Uri>();
            Uri baseUrl = new Uri(BASE_URL);

            foreach (var item in lessons)
            {
                string path = item.FirstChild.GetAttributeValue("href", string.Empty);
                urls.Add(new Uri(baseUrl, path));
            }

            return urls;
        }

        static async Task<IEnumerable<Uri>> GetPowerPointList(string url)
        {
            HtmlDocument doc = await htmlWeb.LoadPageAsync(url);

            var mainFrame = doc.DocumentNode.Descendants("div").First(d => d.HasClass("frame-main"));
            var content = mainFrame.Descendants("div").First(d => d.HasClass("content"));
            var pps = content.Elements("li");

            List<Uri> urls = new List<Uri>();
            Uri baseUrl = new Uri(BASE_URL);

            foreach (var item in pps)
            {
                string path = item.FirstChild.GetAttributeValue("href", string.Empty);
                urls.Add(new Uri(baseUrl, path));
            }

            return urls;
        }

        static async Task<string> ChooseFavoredAuthorOrMaxDownloadPP(IEnumerable<Uri> pps, string favoredAuthor)
        {
            favoredAuthor = favoredAuthor.Trim().ToLower();
            string chosenPP = string.Empty;

            int maxDl = 0;
            foreach (var item in pps)
            {
                HtmlDocument doc = await htmlWeb.LoadPageAsync(item.AbsoluteUri);
                string title = doc.DocumentNode.SelectSingleNode("//head/title").InnerText.ToLower();

                if (!favoredAuthor.Equals(string.Empty) && title.Contains(favoredAuthor))
                {
                    chosenPP = item.AbsoluteUri;
                    break;
                }

                var span = doc.DocumentNode.Descendants("b").First(p => p.InnerText.Equals("Số lượt tải: "));
                string downloadCntStr = span.NextSibling.InnerText;
                int dl = int.Parse(downloadCntStr);
                if (dl > maxDl)
                {
                    maxDl = dl;
                    chosenPP = item.AbsoluteUri;
                }
            }

            return chosenPP;
        }

        static async Task SavePP(string ppUrl, string dirname)
        {
            HtmlDocument doc = await htmlWeb.LoadPageAsync(ppUrl);

            var mainFrame = doc.DocumentNode.Descendants("div").First(d => d.HasClass("frame-main"));
            string title = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
            var content = mainFrame.Descendants("div").First(d => d.HasClass("doc"));
            var h2 = content.Element("h2");
            var anchor = h2.Element("label").Element("a");

            string jsCode = anchor.GetAttributeValue("onclick", string.Empty);
            string id = jsCode.Substring(68, 7);

            string url = $"{BASE_URL}present/download/pr_id/{id}/t/{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            htmlWeb.DownloadFile(url, dirname);

            Console.WriteLine($"Downloaded: {title}");
        }
    }
}
