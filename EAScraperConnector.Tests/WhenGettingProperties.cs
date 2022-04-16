using DocumentFormat.OpenXml.CustomProperties;
using EAScraperConnector.Controllers;
using EAScraperConnector.Data;
using EAScraperConnector.Dtos;
using EAScraperConnector.Interfaces;
using EAScraperConnector.Models;
using Microsoft.EntityFrameworkCore;
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
        Mock<IEFWrapper>? _efWrapper;
        Mock<IRightMoveScraper>? _rightMoveScraper;
        public EAScraperController _eaScraperController { get; set; }
        public DataContext _dataContext { get; set; }

        IQueryable<Property> dbProperties { get; set; }
        Mock<IAuditWrapper> _auditWrapper;

        [SetUp]
        public void Setup()
        {
            _zooplaScraper = new Mock<IZooplaScraper>();
            _excelSaver = new Mock<IExcelSaver>();
            _efWrapper = new Mock<IEFWrapper>();
            _rightMoveScraper = new Mock<IRightMoveScraper>();
            _eaScraperController = new EAScraperController(_zooplaScraper.Object, _excelSaver.Object, _rightMoveScraper.Object, _efWrapper.Object);
        }

        [Test]
        [TestCase("150000")]
        [TestCase("100000")]
        [TestCase("200000")]
        public async Task Given_query_price_then_returned_properties_should_include_up_to_10Pc_less(string price)
        {
            var link = "unique";
            Given_search_request_with(price);
            Given_no_existing_db_entries_with(link);

            var results = await _eaScraperController.Get(price);
            
            Then_results_should_include_up_to_10pc_cheaper(results, price);
        }


        [Test]
        public async Task Given_search_yields_unique_results_then_properties_should_be_saved_to_the_database_and_audited()
        {
            var price = "150000";
            var link = "123456";
            Given_search_request_with(price);
            Given_no_existing_db_entries_with(link);
            await _eaScraperController.Get(price);            
            Mock.Get(_efWrapper.Object).Verify(x => x.SaveToDB(It.IsAny<List<Property>>()), Times.Once);            
        }

        [Test]
        public async Task Given_search_yields_no_unique_results_then_no_properties_should_be_saved_to_the_database()
        {
            var price = "150000";
            var link = "123456";
            Given_search_request_with(price, link);
            Existing_db_entries_with(link);
            await _eaScraperController.Get(price);
            Mock.Get(_efWrapper.Object).Verify(x => x.SaveToDB(It.IsAny<List<Property>>()), Times.Never);
        }

        private void Given_no_existing_db_entries_with(string link)
        {
            var dbProperties = new List<Property>()
            {
                new Property()
                {
                    Price = 150000,
                    Link = "aDifferentLink"
                },
                new Property()
                {
                    Price = 150000,
                    Link = "aDifferentLink"
                },
            };

            _efWrapper.Setup(x => x.GetFromDB()).ReturnsAsync(dbProperties);
        }

        private void Existing_db_entries_with(string link)
        {
            var dbProperties = new List<Property>()
            {
                new Property()
                {
                    Price = 150000,
                    Link = link
                },
                new Property()
                {
                    Price = 150000,
                    Link = link
                },
            };
            _efWrapper.Setup(x => x.GetFromDB()).ReturnsAsync(dbProperties);
        }

        private void Given_search_request_with(string price, string link="")
        {
            _zooplaScraper.Setup(r => r.GetProperties(It.IsAny<string>())).ReturnsAsync(
                    new List<House>() { new House() { Price = price, Link = String.IsNullOrEmpty(link) ? "123456" : link},
                    new House() { Price = (Calculate10PcOffPrice(Convert.ToInt32(price))).ToString(), Link= String.IsNullOrEmpty(link) ? "567891" : link } });

            _rightMoveScraper.Setup(r => r.GetProperties(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(
                    new List<House>() { new House() { Price = price, Link = String.IsNullOrEmpty(link) ? "123456" : link},
                    new House() { Price = (Calculate10PcOffPrice(Convert.ToInt32(price))).ToString(), Link= String.IsNullOrEmpty(link) ? "567891" : link } });
        }



        private int Calculate10PcOffPrice(int price)
        {
            return price - (price / 100 * 10);
        }


        private void Then_results_should_include_up_to_10pc_cheaper(IEnumerable<House> properties, string price)
        {
            Assert.That(Convert.ToInt32(properties.Max(r => r.Price)), Is.LessThanOrEqualTo(Convert.ToInt32(price)));
            Assert.That(Convert.ToInt32(properties.Min(r => r.Price)), Is.GreaterThanOrEqualTo(Calculate10PcOffPrice(Convert.ToInt32(price))));
        }
    }
}