using AngleSharp.Dom;
using AngleSharp.Io;
using EAScraperConnector.Dtos;
using EAScraperConnector.Interfaces;
using EAScraperConnector.Scrapers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EAScraperConnector.Tests
{

    public class WhenScraping
    {
        Mock<IAngleSharpWrapper>? _angleSharpWrapper;
        AngleSharpWrapper? _angleSharpWrap;
        ZooplaScraper? _zoopla;
        Mock<ILogger<ZooplaScraper>>? _logger;
        Mock<IAuditWrapper>? _auditWrapper;
        RightMoveScraper? _rightMove;

        [SetUp]
        public void Setup()
        {
            _angleSharpWrapper = new Mock<IAngleSharpWrapper>();
            _angleSharpWrap = new AngleSharpWrapper();
            _logger = new Mock<ILogger<ZooplaScraper>>();
            _auditWrapper = new Mock<IAuditWrapper>();
            _zoopla = new ZooplaScraper(_angleSharpWrapper.Object, _logger.Object, _auditWrapper.Object);
            _rightMove = new RightMoveScraper(_angleSharpWrapper.Object, _auditWrapper.Object);
        }

        [Test]
        public async Task WhenScrapingZooplaShouldPopulateHouseAndLogToAudit()
        {
            var fakeElement = await GetFakeDocument(Enums.EstateAgent.Zoopla);
            _angleSharpWrapper?.Setup(r => r.GetSearchResults(It.IsAny<string>(), It.IsAny<RequesterWrapper>())).ReturnsAsync(fakeElement);
            var zoo = await _zoopla.GetProperties("150000");
            Assert.IsNotEmpty(zoo.Select(r => r.Link));
            Assert.IsNotEmpty(zoo.Select(r => r.Area));
            Assert.IsNotEmpty(zoo.Select(r => r.Description));
            Assert.GreaterOrEqual(zoo.Count, 1);
            Assert.IsNotNull(zoo.Select(r => r.Price));
            Assert.IsNotNull(zoo.Select(r => r.MonthlyRepayments));
            Assert.IsNotNull(zoo.Select(r => r.Deposit));
            Mock.Get(_auditWrapper.Object).Verify(x => x.SaveToDB(It.IsAny<Audit>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task WhenScrapingRightMoveShouldPopulateHouse()
        {
            var fakeDocument = await GetFakeDocument(Enums.EstateAgent.RightMove);
            _angleSharpWrapper?.Setup(r => r.GetSearchResults(It.IsAny<string>(), It.IsAny<RequesterWrapper>())).ReturnsAsync(fakeDocument);
            var zoo = await _rightMove.GetProperties("150000");
            Assert.IsNotNull(zoo[0].Link);
            Assert.IsNotNull(zoo[0].Area);
            Assert.IsNotNull(zoo.Select(r => r.Images));

            Assert.IsNotNull(zoo[0].Description);
            //Assert.GreaterOrEqual(zoo.Count, 1);
            //Assert.IsNotNull(zoo[0].Price);

            Mock.Get(_auditWrapper.Object).Verify(x => x.SaveToDB(It.IsAny<Audit>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task WhenScrapingV2ZooplaShouldPopulateHouseAndLogToAudit()
        {
            var fakeElement = await GetFakeDocument(Enums.EstateAgent.Zoopla, 2);
            _angleSharpWrapper?.Setup(r => r.GetSearchResults(It.IsAny<string>(), It.IsAny<RequesterWrapper>())).ReturnsAsync(fakeElement);
            var zoo = await _zoopla.GetProperties("150000", true, 2);
            Assert.IsNotNull(zoo[0].Link);
            Assert.IsNotNull(zoo[0].Area);
            Assert.IsNotNull(zoo.Select(r => r.Images));

            Assert.IsNotNull(zoo[0].Description);
            Assert.GreaterOrEqual(zoo.Count, 1);
            Assert.IsNotNull(zoo[0].Price);

            Mock.Get(_auditWrapper.Object).Verify(x => x.SaveToDB(It.IsAny<Audit>()), Times.AtLeastOnce);
        }

        private async Task<IDocument> GetFakeDocument(Enums.EstateAgent estateAgent, int version = 1)
        {
            var requesterMock = GetFakeRequesterMock(estateAgent, version);
            return await _angleSharpWrap.GetSearchResults("http://askjdkaj", requesterMock.Object);
        }

        private Mock<RequesterWrapper> GetFakeRequesterMock(Enums.EstateAgent estateAgent, int version = 1)
        {
            var mockResponse = new Mock<IResponse>();
            mockResponse.Setup(x => x.Address).Returns(new Url("fakeAddress"));
            mockResponse.Setup(x => x.Headers).Returns(new Dictionary<string, string>());
            mockResponse.Setup(_ => _.Content).Returns(LoadFakeDocumentFromField(estateAgent, version));

            var mockFakeRequester = new Mock<RequesterWrapper>();
            mockFakeRequester.Setup(_ => _.RequestAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse.Object);
            mockFakeRequester.Setup(x => x.SupportsProtocol(It.IsAny<string>())).Returns(true);
            return mockFakeRequester;
        }

        private Stream LoadFakeDocumentFromField(Enums.EstateAgent estateAgent, int version = 1)
        {
            var docName = estateAgent == Enums.EstateAgent.RightMove ? "MockRMDocument.html" : "v1/MockSearchResults.html";

            if (version == 2)
            {
                docName = "v2/Zoopla.html";
            }

            var doc = $"TestHtmlDoc/{docName}";

            using (FileStream fileStream = File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), $"{doc}")))
            {
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.SetLength(fileStream.Length);
                fileStream.Read(memoryStream.GetBuffer(), 0, (int)fileStream.Length);

                return memoryStream;
            }
        }
    }

}
