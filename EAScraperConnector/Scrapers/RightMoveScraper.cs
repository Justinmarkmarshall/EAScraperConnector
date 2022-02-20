using EAScraperConnector.Interfaces;
using EAScraperConnector.Models;

namespace EAScraperConnector.Scrapers
{
    public class RightMoveScraper : IRightMoveScraper
    {
        IAngleSharpWrapper _angleSharpWrapper;

        public RightMoveScraper(IAngleSharpWrapper angleSharpWrapper)
        {
            _angleSharpWrapper = angleSharpWrapper;
        }
        public async Task<IList<House>> GetProperties(string price)
        {
            string url = $"https://www.rightmove.co.uk/property-for-sale/find.html?searchType=SALE&locationIdentifier=REGION%5E87490&insId=1&radius=10.0&minPrice={Calculate10PcOffPrice(Convert.ToInt32(price))}&maxPrice={price}&minBedrooms=0&maxBedrooms=1&displayPropertyType=flats&maxDaysSinceAdded=&_includeSSTC=on&sortByPriceDescending=&primaryDisplayPropertyType=&secondaryDisplayPropertyType=&oldDisplayPropertyType=&oldPrimaryDisplayPropertyType=&newHome=&auction=false";
            var document = await _angleSharpWrapper.GetSearchResults(url);
            var searchResults = document.Body.InnerHtml;

            var searchResultsTwo = document.GetElementsByClassName("l-propertySearch-results propertySearch-results");

            //working from live site but not from unit tests
            var searchResultsThree = document.GetElementsByClassName("l-searchResults");

            var foobar = 0;
            return new List<House>();
        }

        private int Calculate10PcOffPrice(int price) => price - (price / 100 * 10);
    }
}
