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

        //Gets the company information based on given stock symbol
        //Result : { ... }
        //Error(no api key) : {error : ""}
        //Error(bad symbol) : { }
        #region GetCompanyProfile

        /// <summary>
        /// Sucessful api call with proper api key and proper symbol
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetCompanyProfile_ValidInput_ToBeSucessful()
        {
            Dictionary<string,object> mockResult = _fixture.Create<Dictionary<string,object>>();           

            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("testApiKey");
            _finnhubRepositoryMock.Setup(x => x.GetCompanyProfile(It.IsAny<string>())).ReturnsAsync(mockResult);

            Dictionary<string, object>? result = await _finnhubService.GetCompanyProfile("AAPL");
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        /// <summary>
        /// Exception when no api is provided in configuration before api call
        /// </summary>
        /// <returns>Exception</returns>
        [Fact]
        public async Task GetCompanyProfile_NoApiKeyProvided_ToThrowException()
        {
            
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                Dictionary<string, object>? result = await _finnhubService.GetCompanyProfile("AAPL");
            });
        }

        /// <summary>
        /// Argument exception when no stock symbol is provied before api call
        /// </summary>
        /// <returns>Argument Exception</returns>
        [Fact]
        public async Task GetCompanyProfile_NoStockSymbolProvided_ToThrowArgumentException()
        {
            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("testApiKey");
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                Dictionary<string, object>? result = await _finnhubService.GetCompanyProfile("");
            });
        }

        /// <summary>
        /// Throws invalid operation exception when supplied with bad api key
        /// </summary>
        /// <returns>Invalid Operation Exception</returns>
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

        /// <summary>
        /// Throws invalid operation exception when supplied with bad symbol
        /// </summary>
        /// <returns>Invalid Operation Exception</returns>
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

        //Gets the price info of specified stock
        //Result : { c:number , ....}
        //Error(No Api key) : {error : ""}
        //Error(Bad Symbol) : { c:0 ... }
        #region GetStockPriceQuote

        /// <summary>
        /// Succesful call with proper api key and result with c value that holds current value
        /// </summary>
        /// <returns>Success</returns>
        [Fact]
        public async Task GetStockPriceQuote_ValidInput_ToBeSucessful()
        {
            Dictionary<string, object> mockResult = _fixture.Create<Dictionary<string, object>>();
            mockResult.Add("c", 100);

            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("testApiKey");
            _finnhubRepositoryMock.Setup(x => x.GetStockPriceQuote(It.IsAny<string>())).ReturnsAsync(mockResult);

            Dictionary<string, object>? result = await _finnhubService.GetStockPriceQuote("AAPL");
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        /// <summary>
        /// Throws exception when no api key is provied in configuration before api call
        /// </summary>
        /// <returns>Exception</returns>
        [Fact]
        public async Task GetStockPriceQuote_NoApiKeyProvided_ToThrowException()
        {

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                Dictionary<string, object>? result = await _finnhubService.GetStockPriceQuote("AAPL");
            });
        }
        
        /// <summary>
        /// Throws argument exception when called with null or empty symbol
        /// </summary>
        /// <returns>Argument exception</returns>
        [Fact]
        public async Task GetStockPriceQuote_NoStockSymbolProvided_ToThrowArgumentException()
        {
            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("testApiKey");
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                Dictionary<string, object>? result = await _finnhubService.GetStockPriceQuote("");
            });
        }
        
        /// <summary>
        /// Throws invalid operation exception when bad api key is provided
        /// </summary>
        /// <returns>Invalid operation exception</returns>
        [Fact]
        public async Task GetStockPriceQuote_BadApiKey_InvalidOperationException()
        {
            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("testApiKey");
            Dictionary<string, object> resultMock = new Dictionary<string, object>()
            {
                {"error","please provide api key" }
            };
            _finnhubRepositoryMock.Setup(x => x.GetStockPriceQuote(It.IsAny<string>())).ReturnsAsync(resultMock);
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                Dictionary<string, object>? result = await _finnhubService.GetCompanyProfile("AAPL");
            });
        }
        
        /// <summary>
        /// Throws invalid operation exception when bad symbol is sent
        /// </summary>
        /// <returns>Invalid Operation Exception</returns>
        [Fact]
        public async Task GetStockPriceQuote_BadSymbol_InvalidOperationException()
        {
            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("testApiKey");
            Dictionary<string, object> resultMock = new Dictionary<string, object>();
            resultMock.Add("c", 0);

            _finnhubRepositoryMock.Setup(x => x.GetStockPriceQuote(It.IsAny<string>())).ReturnsAsync(resultMock);
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                Dictionary<string, object>? result = await _finnhubService.GetCompanyProfile("\"AAPLMistake\"");
            });
        }

        #endregion

        //Gets all stocks that api provides
        //Result : [ {object} , ...]
        //Error : {error : "" }
        #region GetStocks

        /// <summary>
        /// Valid input with api key
        /// </summary>
        /// <returns>Success</returns>
        [Fact]
        public async Task GetStocks_ValidInput_ToBeSucessful()
        {
            List<Dictionary<string, string>> resultMock = _fixture.Create<List<Dictionary<string, string>>>();

            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("TestApiKey");
            _finnhubRepositoryMock.Setup(x => x.GetStocks()).ReturnsAsync(resultMock);

            List<Dictionary<string, string>>? actualResult = await _finnhubService.GetStocks();

            Assert.NotNull(actualResult);
            Assert.NotEmpty(actualResult);
        }

        /// <summary>
        /// No api key provided throws an exception before api call
        /// </summary>
        /// <returns>Exception</returns>
        [Fact]
        public async Task GetStocks_NoApiKeyProvided_ToThrowException()
        {
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _finnhubService.GetStocks();
            });
        }

        #endregion

        //Search stocks : Performs the search with given stock symbol
        //Result : { count : num , result : [...] }
        //Error : {error : ""}
        #region SearchStocks

        /// <summary>
        /// Valid input with api key and proper symbol
        /// Result of api call is { count: number , result : [...] }
        /// </summary>
        /// <returns>Checks the return dictionary</returns>
        [Fact]        
        public async Task SearchStocks_ValidInput_ToBeSucessful()
        {
            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("TestApiKey");
            Dictionary<string,object> mockResult = _fixture.Create<Dictionary<string, object>>();
            _finnhubRepositoryMock.Setup(x => x.SearchStocks(It.IsAny<string>())).ReturnsAsync(mockResult);

            Dictionary<string, object>? actualResult = await _finnhubService.SearchStocks("AAPL");

            Assert.NotNull(actualResult);
            Assert.NotEmpty(actualResult);
            Assert.True(actualResult.Count() > 0);
        }

        /// <summary>
        /// No api key provided, service performs the check before using repository
        /// </summary>
        /// <returns>Exception</returns>
        [Fact]
        public async Task SearchStocks_NoApiKey_ToThrowException()
        {
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _finnhubService.SearchStocks("AAPL");
            });
        }

        /// <summary>
        /// If stock symbol is null or empty service throws Argument exception
        /// </summary>
        /// <returns>Argument Exception</returns>
        [Fact]
        public async Task SearchStocks_NoStockSymbolProvied_ToThrowArgumentException()
        {
            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("TestApiKey");

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _finnhubService.SearchStocks("");
            });
        }

        /// <summary>
        /// If the provided api key is bad the result is in error
        /// </summary>
        /// <returns>Invalid Operation Exception</returns>
        [Fact]
        public async Task SearchStocks_BadApiKey_ToThrowInvalidOperationException()
        {
            _configurationMock.Setup(x => x["FinnhubApiKey"]).Returns("TestApiKey");
            Dictionary<string, object> mockResult = new Dictionary<string, object>()
            {
                {"error","Invalid api key" }
            };
            _finnhubRepositoryMock.Setup(x => x.SearchStocks(It.IsAny<string>())).ReturnsAsync(mockResult);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _finnhubService.SearchStocks("AAPL");
            });
        }

        #endregion
    }
}
