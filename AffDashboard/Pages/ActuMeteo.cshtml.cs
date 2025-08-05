using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace AffDashboard.Pages
{
    public class ActuMeteoModel : PageModel
    {
        public RssArticle? BfmArticle { get; set; }      
        public RssArticle? France24Article { get; set; }   

        public async Task OnGetAsync()
        {
            BfmArticle = await GetLatestArticle("https://www.bfmtv.com/rss/news-24-7/"); 
            France24Article = await GetLatestArticle("https://www.france24.com/fr/france/rss");  
        }

        //fonction partagée pour les 2 flux
        private async Task<RssArticle?> GetLatestArticle(string feedUrl)
        {
            using var httpClient = new HttpClient();
            var feedData = await httpClient.GetStringAsync(feedUrl);

            var feed = XDocument.Parse(feedData);
            var item = feed.Descendants("item").FirstOrDefault();
            if (item == null) return null;

            var title = item.Element("title")?.Value ?? "";
            var description = item.Element("description")?.Value ?? "";
            var link = item.Element("link")?.Value ?? "";

            string imageUrl = "";

            // Balise media:content
            XNamespace media = "http://search.yahoo.com/mrss/";
            var mediaContent = item.Element(media + "content");
            if (mediaContent != null)
            {
                imageUrl = mediaContent.Attribute("url")?.Value ?? "";
            }

            // <img> dans la description
            if (string.IsNullOrEmpty(imageUrl))
            {
                var imgMatch = Regex.Match(description, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
                if (imgMatch.Success)
                    imageUrl = imgMatch.Groups[1].Value;
            }

            var plainDescription = Regex.Replace(description, "<.*?>", "").Trim();

            return new RssArticle
            {
                Title = title,
                Summary = plainDescription,
                Link = link,
                ImageUrl = imageUrl
            };
        }
    }

        public class RssArticle
        {
            public string Title { get; set; }
            public string Summary { get; set; }
            public string Link { get; set; }
            public string ImageUrl { get; set; }
        }
}

