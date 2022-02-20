using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using EAScraperConnector.Interfaces;
using EAScraperConnector.Mappers;
using EAScraperConnector.Models;

namespace EAScraperConnector.Scrapers
{
    //div class which contains the search results css-8jz4jb-SearchResultsLayoutGroup es0c9wm6
    //div class with listing containers css-1anhqz4-ListingsContainer earci3d2
    //div class for individual search results search-result_listing_60796732
    //parentNode children html has the juice
    //private const string zooplaUrl = "https://www.zoopla.co.uk/for-sale/flats/ealing-common/?is_auction=false&is_shared_ownership=false&page_size=25&price_max=150000&view_type=list&q=Ealing%20Common&radius=10&results_sort=newest_listings&search_source=facets";
    public class ZooplaScraper : IZooplaScraper
    {
        IAngleSharpWrapper _angleSharpWrapper;
        ILogger<ZooplaScraper> _logger;
        private readonly List<string> _locations = new List<string>() { "Ealing-Common", "London" };

        public ZooplaScraper(IAngleSharpWrapper angleSharpWrapper, ILogger<ZooplaScraper> logger)
        {
            _angleSharpWrapper = angleSharpWrapper;
            _logger = logger;
        }

        public async Task<IList<House>> GetProperties(string price)
        {
            var lstReturn = new List<House>();

            foreach (var location in _locations)
            {
                string url = $"https://www.zoopla.co.uk/for-sale/flats/{location.ToLower()}/?is_auction=false&is_shared_ownership=false&page_size=25&price_max={price}&price_min={Calculate10PcOffPrice(Convert.ToInt32(price))}&view_type=list&q={location.Replace("-", "%")}&radius=10&results_sort=newest_listings&search_source=facets";
                var document = await _angleSharpWrapper.GetSearchResults(url);
                var searchResults = document.GetElementsByClassName("css-1anhqz4-ListingsContainer earci3d2");
                if (searchResults.Any())
                {
                    var newHomes = searchResults.Map();
                    foreach (var home in newHomes)
                    {
                        if (!lstReturn.Any(r => r.Link == home.Link)) lstReturn.Add(home);
                    }
                }
            }
            
            return lstReturn;
        }

        private int Calculate10PcOffPrice(int price) => price - (price / 100 * 10);

    }
}
