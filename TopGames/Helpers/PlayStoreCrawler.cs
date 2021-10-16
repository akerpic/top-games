using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using TopGames.Models;

namespace TopGames.Helpers
{
    public class PlayStoreCrawler : IStoreCrawler
    {
        private readonly ILogger<PlayStoreCrawler> _logger;

        private const string TopGamesUrl = @"https://play.google.com/store/apps/collection/cluster?clp=0g4cChoKFHRvcHNlbGxpbmdfZnJlZV9HQU1FEAcYAw%3D%3D:S:ANO1ljJ_Y5U&gsr=Ch_SDhwKGgoUdG9wc2VsbGluZ19mcmVlX0dBTUUQBxgD:S:ANO1ljL4b8c";
        private const string BaseUrl = @"https://play.google.com";


        public PlayStoreCrawler(ILogger<PlayStoreCrawler> logger)
        {
            _logger = logger;
        }

        private async Task<List<string>> GetTopGamesHrefs(int topNumber=10)
        {
            List<string> top10GamesHrefs = new List<string>();
            try{
                var htmlDoc = await GetHtmlDocByUrl(TopGamesUrl);
                var gameNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'Vpfmgd')]");
                for (int i = 0; i < topNumber; i++)
                {
                    top10GamesHrefs.Add(gameNodes[i].Descendants("a").FirstOrDefault().Attributes["href"].Value);
                }
            }
            catch(Exception ex){
                _logger.LogError("GetTopGamesHrefs - " + ex.Message);
            }
            return top10GamesHrefs;
        }

        private static string GetGameUrl(string href)
        {
            return BaseUrl + href;
        }

        private static string GetTrackIdByUrl(string gameUrl)
        {
            var uri = new Uri(gameUrl);
            return HttpUtility.ParseQueryString(uri.Query).Get("id");
        }

        private static async Task<HtmlDocument> GetHtmlDocByUrl(string url)
        {
            HtmlWeb web = new HtmlWeb();
            return await web.LoadFromWebAsync(url);
        }

        private int? GetTotalReviewCount(HtmlDocument htmlDoc)
        {
            try
            {
                var reviewCountStr = htmlDoc.DocumentNode.SelectNodes("//span[contains(@class, 'AYi5wd TBRnV')]").FirstOrDefault().InnerText;
                return Int32.Parse(reviewCountStr.Replace(",", ""));
            }
            catch(ArgumentNullException ex)
            {
                // The HTML Document doesn't contain TotalReviewCount.
                _logger.LogWarning("getTotalReviewCount - " + ex.Message);
                return null;
            }
        }

        private static string GetTitle(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode.SelectNodes("//h1[contains(@itemprop, 'name')]")[0].ChildNodes.FirstOrDefault().InnerHtml;
        }

        private static string GetDescription(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode.SelectNodes("//div[contains(@jsname, 'sngebd')]").FirstOrDefault().InnerText;
        }

        private DateTime GetLastUpdateDate(string lastUpdateDateStr)
        {
            try
            {
                return DateTime.ParseExact(lastUpdateDateStr, "MMMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch(FormatException)
            {
                return DateTime.ParseExact(lastUpdateDateStr, "MMMM d, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        private async Task<Game> GetGameByHref(string href, int ranking)
        {
            try
            {
                var gameUrl = GetGameUrl(href);
                var trackId = GetTrackIdByUrl(gameUrl);
                Game game = new(trackId,ranking);

                var htmlDoc = await GetHtmlDocByUrl(gameUrl);
                IDictionary<string, string> additionalInformation = GetAdditionalInformation(htmlDoc);

                game.title = GetTitle(htmlDoc);
                game.description = GetDescription(htmlDoc);
                game.total_review_count =GetTotalReviewCount(htmlDoc);
                game.last_update_date = GetLastUpdateDate(additionalInformation["Updated"]);
                game.size_in_mb = Int32.Parse(additionalInformation["Size"].Replace("M",""));
                game.total_install_count = long.Parse(additionalInformation["Installs"].Replace("+","").Replace(",",""));
                game.current_version = additionalInformation["Current Version"];
                game.author = additionalInformation["Offered By"];
                return game;
            }
            catch(Exception ex)
            {
                _logger.LogError("GetGameByHref('{href}') - Error while obtaining game information", href);
                _logger.LogError(ex.Message);
                return null;
            }

        }

        private static IDictionary<string, string> GetAdditionalInformation(HtmlDocument htmlDoc)
        {
            try
            {
                var additionalInformationNodes = htmlDoc.DocumentNode.SelectNodes("//c-wiz[contains(@jsrenderer, 'HEOg8')]")[0].ChildNodes[0].ChildNodes[1].ChildNodes[0].ChildNodes;
                IDictionary<string, string> additionalInformations = new Dictionary<string, string>();
                foreach (var node in additionalInformationNodes)
                {
                    if (node.ChildNodes.Count > 1)
                        additionalInformations.Add(node.ChildNodes[0].InnerText, node.ChildNodes[1].InnerText);
                }
                return additionalInformations;
            }
            catch
            {
                throw;
            }

        }

        public async Task<List<Game>> GetTopGames()
        {
            List<Game> topGames = new List<Game>();
            var topGamesHrefs = await GetTopGamesHrefs();
            for (int index = 0; index < topGamesHrefs.Count; index++)
            {
                var game = await GetGameByHref(topGamesHrefs[index], index+1);
                if(game != null)
                    topGames.Add(game);
            }
            return topGames;
        }
    }
}
