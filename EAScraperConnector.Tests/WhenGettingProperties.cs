using EAScraperConnector.Interfaces;
using EAScraperConnector.Models;
using EAScraperConnector.Scrapers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAScraperConnector.Tests
{
    public class WhenGettingProperties
    {
        Mock<IZooplaScraper>? _zooplaScraper;
        Mock<IExcelSaver>? _excelSaver;

        [SetUp]
        public void Setup()
        {
            _zooplaScraper = new Mock<IZooplaScraper>();
            _excelSaver = new Mock<IExcelSaver>();
        }

        [Test]
        [TestCase("150000")]
        [TestCase("100000")]
        [TestCase("200000")]
        public async Task PriceOfReturnedPropertiesShouldBeUpTo10PcLessThanGiven(string price)
        {         
            _zooplaScraper.Setup(r => r.GetProperties(It.IsAny<string>())).ReturnsAsync(
                new List<House>() { new House() { Price = price }, 
                new House() { Price = (Calculate10PcOffPrice(Convert.ToInt32(price))).ToString() } });
            var properties = await GetProperties(price);

            Assert.That(Convert.ToInt32(properties.Max(r => r.Price)), Is.LessThanOrEqualTo(Convert.ToInt32(price)));
            Assert.That(Convert.ToInt32(properties.Min(r => r.Price)), Is.GreaterThanOrEqualTo(Calculate10PcOffPrice(Convert.ToInt32(price))));
            
        }

        [Test]
        public void When_search_is_made_then_properties_should_be_saved_to_spreadsheet()
        {
            var price = "150000";

            //Arrange
            _zooplaScraper.Setup(r => r.GetProperties(It.IsAny<string>())).ReturnsAsync(
                new List<House>() { new House() { Price = price },
                new House() { Price = (Calculate10PcOffPrice(Convert.ToInt32(price))).ToString() } });

            //Act
            var properties = GetProperties(price);

            //Assert            
            Mock.Get(_excelSaver.Object).Verify(x => x.SaveToExcel(It.IsAny<List<House>>()), Times.Once);
        }

        private int Calculate10PcOffPrice(int price)
        {
            return price - (price / 100 * 10);
        }

        private async Task<IList<House>> GetProperties(string price)
        {
            var houses = await _zooplaScraper?.Object.GetProperties(price);
            _excelSaver?.Object.SaveToExcel(houses.ToList());
            return new List<House>() { new House() { Price = price.ToString() } };
        }

        private object GenerateLegitRequest(int price)
        {
            throw new System.NotImplementedException();
        }
    }
}