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
using System.Text;
using System.Threading.Tasks;

namespace EAScraperConnector.Tests
{
    public class WhenSearchingPropertiesBySalary
    {
        Mock<IZooplaScraper>? _zooplaScraper;
        Mock<IExcelSaver>? _excelSaver;
        Mock<IEFWrapper>? _efWrapper;
        Mock<IRightMoveScraper>? _rightMoveScraper;
        public EAScraperController _eaScraperController { get; set; }
        public DataContext _dataContext { get; set; }

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
        public async Task Given_net_salary_disposable_income_and_savings_amount_then_should_return_a_list_of_properties_with_an_achievable_by_date()
        {
            var netMonthly = 2200;
            var grossMonthlySalary = 3000;
            var savingsAmount = 15000;
            var disposableIncome = 600;
            var grossAnnualSalary = grossMonthlySalary * 12;
            var affordableProperty = (grossAnnualSalary * 4) + savingsAmount;           
            Given_valid_netSalary_savingsAmount_and_disposableIncome(netMonthly, grossMonthlySalary, savingsAmount, disposableIncome);
            Given_existing_properties_with_price_four_times_grossMonthlySalary_and_deposit_a_multiple_of_netMonthlySalary(affordableProperty);
            var propertySpecs = await _eaScraperController.BySalary(savingsAmount, grossMonthlySalary, disposableIncome, netMonthly);
            Then_propertySpecs_should_be_within_reach_and_calculate_a_date_when_they_will_be_affordable(propertySpecs.ToList(), affordableProperty);
        }

        private void Then_propertySpecs_should_be_within_reach_and_calculate_a_date_when_they_will_be_affordable(List<PropertySpec> properties, double affordableProperty)
        {
            Assert.That(!properties.Where(r => r.Property.Price > affordableProperty && r.AchievableBy < DateTime.UtcNow.AddYears(1)).Any());
        }
        private void Given_existing_properties_with_price_four_times_grossMonthlySalary_and_deposit_a_multiple_of_netMonthlySalary(double affordableProperty)
        {
            var propertiesByPrice = new List<Property>()
            {
                new Property()
                {
                    Price = affordableProperty
                },
                 new Property()
                {
                    Price = affordableProperty - 5000
                  },
                  new Property()
                {
                    Price = affordableProperty - 10000
                },
                      new Property()
                {
                    Price = affordableProperty
                },
                 new Property()
                {
                    Price = affordableProperty + 5000
                  },
                  new Property()
                {
                    Price = affordableProperty + 10000
                },

            };
            //not sure how to do something like this 
            //_efWrapper.Setup(r => r.GetByPrice(It.IsAny<double>())).ReturnsAsync(new List<Property>() { new Property() { Price = a } });
            _efWrapper.Setup(r => r.GetByPrice(It.IsAny<double>())).ReturnsAsync(propertiesByPrice);
        }
        private void Given_valid_netSalary_savingsAmount_and_disposableIncome(int netMonthlySalary, int grossMonthlySalary, int savingsAmount, int disposableIncome)
        {
            Assert.IsTrue(netMonthlySalary < grossMonthlySalary);
            Assert.IsTrue(disposableIncome < netMonthlySalary);
        }

    }
}
