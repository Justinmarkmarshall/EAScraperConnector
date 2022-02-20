using EAScraperConnector.Interfaces;
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

        public EAScraperController(IZooplaScraper zooplaController, IExcelSaver excelSaver,
            IRightMoveScraper rightMoveScraper)
        {
            _zooplaScraper = zooplaController;
            _excelSaver = excelSaver;
            _rightMoveScraper = rightMoveScraper;
        }

        [HttpGet(Name = "GetProperties")]
        public async Task<IEnumerable<House>> Get(string price)
        {
            var result = await _zooplaScraper.GetProperties(price);
            var rmResult = await _rightMoveScraper.GetProperties(price);
            _excelSaver.SaveToExcel(result.ToList());
            return result;
        }
    }
}
