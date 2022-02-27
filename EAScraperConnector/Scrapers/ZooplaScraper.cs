﻿using AngleSharp;
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
    //private const string zooplaUrl = "https://www.zoopla.co.uk/for-sale/flats/ealing-common/?is_auction=false&is_shared_ownership=false&page_size=25&Londonprice_max=150000&view_type=list&q=Ealing%20Common&radius=10&results_sort=newest_listings&search_source=facets";
    public class ZooplaScraper : IZooplaScraper
    {
        private Dictionary<string, string> _affordablePostCodes = new Dictionary<string, string>()
        {
            { "HemelHempstead", "HP1" },
            { "Beckton", "E6" },
            { "Thamesmead", "SE28" },
            { "UpperEdmonton", "N9" },
            { "AbbeyWood", "SE2" },
            { "South Norwood", "SE25" },
            { "East Ham", "E12" },
            { "Plumstead", "SE18" },
            { "Anerley", "SE20" },
            { "Penge", "BR3" },
            { "Plaistow", "E13" },
            { "Peterborough", "PE1" },
            { "Purfleet", "RM19" },
            { "Wellingborough", "NN29" },
            { "Rugby", "CV21" },
            { "Chatham", "ME1" },
            { "Hatfield", "AL10" },
            { "Reading", "RG1" },
            { "Borehamwood", "WD6" },
            { "Iver", "SL0" },
            { "Acton", "W3" }
        };

        IAngleSharpWrapper _angleSharpWrapper;
        ILogger<ZooplaScraper> _logger;
        private readonly List<string> _locations = new List<string>() { "Ealing-Common", "London", "HP1", "E6", "SE28" };
        IAuditWrapper _auditWrapper;

        public ZooplaScraper(IAngleSharpWrapper angleSharpWrapper, ILogger<ZooplaScraper> logger, IAuditWrapper auditWrapper)
        {
            _angleSharpWrapper = angleSharpWrapper;
            _logger = logger;
            _auditWrapper = auditWrapper;
        }

        public async Task<IList<House>> GetProperties(string price)
        {
            var uniqueHouses = new List<House>();

            foreach (var location in _affordablePostCodes)
            {
                var postCodeCounter = 0;
                string url = $"https://www.zoopla.co.uk/for-sale/flats/{location.Value.ToLower()}/?is_auction=false&is_shared_ownership=false&page_size=25&price_max={price}&price_min={Calculate10PcOffPrice(Convert.ToInt32(price))}&view_type=list&q={location.Value.Replace("-", "%")}&radius=15&results_sort=newest_listings&search_source=facets";
                var document = await _angleSharpWrapper.GetSearchResults(url);
                var searchResults = document.GetElementsByClassName("css-1anhqz4-ListingsContainer earci3d2");
                if (searchResults.Any())
                {                   
                    var newHomes = searchResults.MapZ();
                    foreach (var home in newHomes)
                    {
                        if (!uniqueHouses.Any(r => r.Link == home.Link)) {
                            uniqueHouses.Add(home);
                            postCodeCounter++;
                                };
                    }
                }
                await _auditWrapper.SaveToDB(postCodeCounter.Map(location.Value, price, Enums.EstateAgent.Zoopla));                
            }
            
            return uniqueHouses;
        }

        private int Calculate10PcOffPrice(int price) => price - (price / 100 * 10);

    }
}
