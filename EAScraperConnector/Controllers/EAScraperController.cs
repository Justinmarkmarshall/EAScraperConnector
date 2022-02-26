using EAScraperConnector.Interfaces;
using EAScraperConnector.Mappers;
using EAScraperConnector.Models;
using Microsoft.AspNetCore.Mvc;

namespace EAScraperConnector.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EAScraperController : Controller
    {
        private IRightMoveScraper _rightMoveScraper;
        private IZooplaScraper _zooplaScraper;
        private IExcelSaver _excelSaver;
        private IEFWrapper _efWrapper;

        public EAScraperController(IZooplaScraper zooplaController, IExcelSaver excelSaver,
            IRightMoveScraper rightMoveScraper, IEFWrapper EFWrapper)
        {
            _zooplaScraper = zooplaController;
            _excelSaver = excelSaver;
            _rightMoveScraper = rightMoveScraper;
            _efWrapper = EFWrapper;
        }

        [HttpGet(Name = "GetProperties")]
        public async Task<IEnumerable<House>> Get(string price)
        {
            var results = await _zooplaScraper.GetProperties(price);
            if (results.Any())
            {
                var uniqueResults = await RemoveDuplicates(results.ToList());

                if (uniqueResults.Any())
                {
                    await _efWrapper.SaveToDB(uniqueResults.Map());
                    return uniqueResults;
                }
            }

            return new List<House>();
        }

        private async Task<List<House>> RemoveDuplicates(List<House> houses)
        {
            var existingProperties = await _efWrapper.GetFromDB();

            var uniqueHomes = new List<House>();

            foreach (var house in houses)
            {
                if (!existingProperties.Select(r => r.Link).Contains(house.Link))
                {
                    uniqueHomes.Add(house);
                }
            }
            return uniqueHomes;
        }
    }
}
