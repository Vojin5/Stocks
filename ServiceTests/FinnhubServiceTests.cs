using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests
{
    public class FinnhubServiceTests
    {
        private readonly IFinnhubRepository _finnhubRepository;
        private readonly IConfiguration _configuration;

        private readonly Mock<IFinnhubRepository> _finnhubRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;

        private readonly IFinnhubService _finnhubService;

        private readonly IFixture _fixture;
        public FinnhubServiceTests()
        {
            _finnhubRepositoryMock = new Mock<IFinnhubRepository>();
            _finnhubRepository = _finnhubRepositoryMock.Object;

            _configurationMock = new Mock<IConfiguration>();
            _configuration = _configurationMock.Object;

            _fixture = new Fixture();

            _finnhubService = new FinnhubService(_finnhubRepository,_configuration);
        }

        #region GetCompanyProfile

        [Fact]
        public async Task GetCompanyProfile_ValidResponse_ToBeSucessful()
        {
            Dictionary<string,object> mockResult = _fixture.Create<Dictionary<string,object>>();           

            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("testApiKey");
            _finnhubRepositoryMock.Setup(x => x.GetCompanyProfile(It.IsAny<string>())).ReturnsAsync(mockResult);

            Dictionary<string, object>? result = await _finnhubService.GetCompanyProfile("AAPL");
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }


        [Fact]
        public async Task GetCompanyProfile_NoApiKeyProvided_ToThrowException()
        {
            
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                Dictionary<string, object>? result = await _finnhubService.GetCompanyProfile("AAPL");
            });
        }

        [Fact]
        public async Task GetCompanyProfile_NoStockSymbolProvided_ToThrowArgumentException()
        {
            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("testApiKey");
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                Dictionary<string, object>? result = await _finnhubService.GetCompanyProfile("");
            });
        }

        [Fact]
        public async Task GetCompanyProfile_BadApiKey_InvalidOperationException()
        {
            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("testApiKey");
            Dictionary<string, object> resultMock = new Dictionary<string, object>()
            {
                {"error","please provide api key" }
            };
            _finnhubRepositoryMock.Setup(x => x.GetCompanyProfile(It.IsAny<string>())).ReturnsAsync(resultMock);
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                Dictionary<string, object>? result = await _finnhubService.GetCompanyProfile("AAPL");
            });
        }

        [Fact]
        public async Task GetCompanyProfile_BadSymbol_InvalidOperationException()
        {
            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("testApiKey");
            Dictionary<string, object> resultMock = new Dictionary<string, object>();

            _finnhubRepositoryMock.Setup(x => x.GetCompanyProfile(It.IsAny<string>())).ReturnsAsync(resultMock);
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                Dictionary<string, object>? result = await _finnhubService.GetCompanyProfile("\"AAPLMistake\"");
            });
        }

        #endregion
    }
}
