using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace LinkLogger.Controllers.Api
{
    public class HtmlHelpers
    {
        public static async Task<string> FetchTitle(string url)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.Content.Headers.ContentType.MediaType.StartsWith("text/"))
            {
                using (var htmlStream = await response.Content.ReadAsStreamAsync())
                {
                    var htmlDoc = new HtmlDocument();
                    var encoding = GetEncoding(response.Content.Headers.ContentType.CharSet);
                    htmlDoc.Load(htmlStream, encoding);

                    string title;

                    var ogTitleElement = htmlDoc.DocumentNode.SelectSingleNode("/html/head/meta[@property='og:title']");
                    if (ogTitleElement != null)
                    {
                        title = ogTitleElement.GetAttributeValue("content", null);

                    }
                    else
                    {
                        var titleElement = htmlDoc.DocumentNode.SelectSingleNode("/html/head/title");
                        title = titleElement.InnerText;
                    }

                    return WebUtility.HtmlDecode(title);
                }
            }
            return null;
        }

        public static bool IsImage(string url)
        {
            var uri = new Uri(url);
            var r = new Regex("^/[\\w\\s./]+\\.(?<extension>gif|png|jpg|jpeg)$", RegexOptions.IgnoreCase);
            return r.IsMatch(uri.LocalPath);
        }

        private static Encoding GetEncoding(string htmlContentTypeCharset)
        {
            if(htmlContentTypeCharset == null) return Encoding.UTF8;

            Encoding encoding;
            if (htmlContentTypeCharset.Equals("utf-8", StringComparison.InvariantCultureIgnoreCase))
                encoding = Encoding.UTF8;
            else
                encoding = Encoding.UTF7;
            return encoding;
        }
    }
}