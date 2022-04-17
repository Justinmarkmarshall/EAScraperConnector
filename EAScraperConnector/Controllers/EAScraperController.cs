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
        private readonly Dictionary<int, int> _predictedSalaryIncreases = new Dictionary<int, int>() { { 1, 40000 }, { 2, 45000 } };

        public EAScraperController(IZooplaScraper zooplaController, IExcelSaver excelSaver,
            IRightMoveScraper rightMoveScraper, IEFWrapper EFWrapper)
        {
            _zooplaScraper = zooplaController;
            _excelSaver = excelSaver;
            _rightMoveScraper = rightMoveScraper;
            _efWrapper = EFWrapper;
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IEnumerable<House>> Get(string price)
        {
            var rmResults = await _rightMoveScraper.GetProperties(price);
            var zoopResults = await _zooplaScraper.GetProperties(price);

            var results = new List<House>();
            results.AddRange(rmResults);
            results.AddRange(zoopResults);

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

        [HttpGet]
        [Route("Scrape")]
        public async Task<IEnumerable<House>> Scrape(string price, bool londonOnly)
        {
            var rmResults = await _rightMoveScraper.GetProperties(price, londonOnly);
            var zoopResults = await _zooplaScraper.GetProperties(price, londonOnly, 2);

            var results = new List<House>();
            results.AddRange(rmResults);
            results.AddRange(zoopResults);
            return results;
        }

        [HttpGet]
        [Route("Salary")]
        public async Task<IEnumerable<PropertySpec>> BySalary(int savings, int grossMonthlyIncome, int disposableIncome, int netMonthly)
        {
            
            var propertySpecs = new List<PropertySpec>();
            for (int i = 0; i < 2; i++)
            {
                var (newSavings, newGrossMonthly, futureDisposable) = NormaliseForTimePassed(i, netMonthly, grossMonthlyIncome, disposableIncome, savings);
                var affordableProperty = ((newGrossMonthly * 12) * 4) + newSavings;
                var properties = await _efWrapper.GetByPrice(Convert.ToDouble(affordableProperty));
                foreach (var property in properties.OrderByDescending(p => p.Date).Take(25))
                {
                    propertySpecs.Add(new PropertySpec()
                    {
                        Property = property,
                        AchievableBy = CalculateAchievableBy(futureDisposable, newSavings, property.Price, i)
                    });
                }
               
            }
            return propertySpecs;

        }

        private Tuple<int, int, int> NormaliseForTimePassed(int numberOfYears, int netMonthly, int grossMonthly, 
            int disposable, int savings)
        {
            var futureNetMonthlySalary = netMonthly + (300 * numberOfYears);
            var futureGrossMonthly = grossMonthly + (416 * numberOfYears);
            var futureDisposable = disposable + (300 * numberOfYears);
            var futureSavings = (futureDisposable * (12 * numberOfYears)) + savings;
            return new Tuple<int, int, int>(futureSavings, futureGrossMonthly, futureDisposable);
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

        private DateTime CalculateAchievableBy(int disposableIncome, int savings, double price, int i)
        {
            var deposit = price / 10;

            var outstandingRequired = deposit - savings;

            //need a calculation like this

            var monthsLeft = 0D;

            if (outstandingRequired > 0)
            {
                monthsLeft = Math.Ceiling((outstandingRequired / disposableIncome));
            }

            var foo = DateTime.UtcNow.AddYears(i).AddMonths((int)monthsLeft);

            return foo;

        }
    }

}
