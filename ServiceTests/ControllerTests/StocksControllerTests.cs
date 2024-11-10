using AutoFixture;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using Services;
using StocksApp.ConfiguraitonOptions;
using StocksApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests.ControllerTests
{
    public class StocksControllerTests
    {
        private readonly IStocksService _stocksService;
        private readonly IFinnhubService _finnhubService;
        private readonly IOptions<TradingOptions> _tradingOptions;
        private readonly IMemoryCache _memoryCache;

        private readonly Mock<IStocksService> _stocksServiceMock;
        private readonly Mock<IFinnhubService> _finnhubServiceMock;
        private readonly Mock<IOptions<TradingOptions>> _optionsMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;

        private readonly IFixture _fixture;

        public StocksControllerTests()
        {
            _finnhubServiceMock = new Mock<IFinnhubService>();
            _stocksServiceMock = new Mock<IStocksService>();
            _optionsMock = new Mock<IOptions<TradingOptions>>();
            _memoryCacheMock = new Mock<IMemoryCache>();

            _stocksService = _stocksServiceMock.Object;
            _finnhubService = _finnhubServiceMock.Object;
            _tradingOptions = _optionsMock.Object;
            _memoryCache = _memoryCacheMock.Object;

            _fixture = new Fixture();
        }

        //Returns the page with list of available stocks
        //Selection of each stock for detail display
        //Trade option of selected stock -> redirects to index page with selected stock
        #region Explore

        /// <summary>
        /// Calling Explore with providing options and return api call
        /// </summary>
        /// <returns>Explore View</returns>
        [Fact]
        public async Task Explore_ValidInput_ToBeSucessful()
        {
            TradingOptions tradingOptionsMock = new TradingOptions();
            tradingOptionsMock.Top25PopularStocks = new List<string>()
            {
                "AAPL", "MSFT", "AMZN", "TSLA", "GOOGL", "GOOG", "NVDA", "BRK.B", "META", "UNH", "JNJ", "JPM", "V", "PG", "XOM", "HD", "CVX", "MA", "BAC", "ABBV", "PFE", "AVGO", "COST", "DIS", "KO"
            };
            _optionsMock.Setup(x => x.Value).Returns(tradingOptionsMock);

            List<Dictionary<string, string>> apiMock = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string>(){{"symbol","AAPL"},{"description","Apple"}},
                new Dictionary<string, string>(){{"symbol","MSFT"},{"description","Microsoft"}},
                new Dictionary<string, string>(){{"symbol","NVDA" },{"description","Nvidia"}}
            };
            _finnhubServiceMock.Setup(x => x.GetStocks()).ReturnsAsync(apiMock);
            var cacheEntryMock = new Mock<ICacheEntry>();
            _memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);
            cacheEntryMock.Setup(x => x.Value).Returns(apiMock);

            StocksController controller = new StocksController(_tradingOptions, _finnhubService, _stocksService,_memoryCache);

            IActionResult result = await controller.Explore(null);
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<StockViewModel>>(viewResult.Model);
        }
        /// <summary>
        /// No Options provided
        /// </summary>
        /// <returns>View with errors</returns>
        [Fact]
        public async Task Explore_NoOptions_ToHaveErrors()
        {
            /*TradingOptions tradingOptionsMock = new TradingOptions();
            tradingOptionsMock.Top25PopularStocks = new List<string>()
            {
                "AAPL", "MSFT", "AMZN", "TSLA", "GOOGL", "GOOG", "NVDA", "BRK.B", "META", "UNH", "JNJ", "JPM", "V", "PG", "XOM", "HD", "CVX", "MA", "BAC", "ABBV", "PFE", "AVGO", "COST", "DIS", "KO"
            };
            _optionsMock.Setup(x => x.Value).Returns(tradingOptionsMock);*/

            List<Dictionary<string, string>> apiMock = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string>(){{"symbol","AAPL"},{"description","Apple"}},
                new Dictionary<string, string>(){{"symbol","MSFT"},{"description","Microsoft"}},
                new Dictionary<string, string>(){{"symbol","NVDA" },{"description","Nvidia"}}
            };
            _finnhubServiceMock.Setup(x => x.GetStocks()).ReturnsAsync(apiMock);

            StocksController controller = new StocksController(_tradingOptions, _finnhubService, _stocksService, _memoryCache);

            IActionResult result = await controller.Explore(null);
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData["Errors"]);
        }

        [Fact]
        public async Task Explore_NoApiReturn_ToHaveErrors()
        {
            TradingOptions tradingOptionsMock = new TradingOptions();
            tradingOptionsMock.Top25PopularStocks = new List<string>()
            {
                "AAPL", "MSFT", "AMZN", "TSLA", "GOOGL", "GOOG", "NVDA", "BRK.B", "META", "UNH", "JNJ", "JPM", "V", "PG", "XOM", "HD", "CVX", "MA", "BAC", "ABBV", "PFE", "AVGO", "COST", "DIS", "KO"
            };
            _optionsMock.Setup(x => x.Value).Returns(tradingOptionsMock);

            /*List<Dictionary<string, string>> apiMock = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string>(){{"symbol","AAPL"},{"description","Apple"}},
                new Dictionary<string, string>(){{"symbol","MSFT"},{"description","Microsoft"}},
                new Dictionary<string, string>(){{"symbol","NVDA" },{"description","Nvidia"}}
            };
            _finnhubServiceMock.Setup(x => x.GetStocks()).ReturnsAsync(apiMock);*/
            var cacheEntryMock = new Mock<ICacheEntry>();
            _memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);
            cacheEntryMock.Setup(x => x.Value).Returns(null as Object);

            StocksController controller = new StocksController(_tradingOptions, _finnhubService, _stocksService, _memoryCache);

            IActionResult result = await controller.Explore(null);
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData["Errors"]);
        }

        [Fact]
        public async Task Explore_ValidInputWithSymbol_ToBeSucessful()
        {
            TradingOptions tradingOptionsMock = new TradingOptions();
            tradingOptionsMock.Top25PopularStocks = new List<string>()
            {
                "AAPL", "MSFT", "AMZN", "TSLA", "GOOGL", "GOOG", "NVDA", "BRK.B", "META", "UNH", "JNJ", "JPM", "V", "PG", "XOM", "HD", "CVX", "MA", "BAC", "ABBV", "PFE", "AVGO", "COST", "DIS", "KO"
            };
            _optionsMock.Setup(x => x.Value).Returns(tradingOptionsMock);

            List<Dictionary<string, string>> apiMock = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string>(){{"symbol","AAPL"},{"description","Apple"}},
                new Dictionary<string, string>(){{"symbol","MSFT"},{"description","Microsoft"}},
                new Dictionary<string, string>(){{"symbol","NVDA" },{"description","Nvidia"}}
            };
            _finnhubServiceMock.Setup(x => x.GetStocks()).ReturnsAsync(apiMock);
            
            var cacheEntryMock = new Mock<ICacheEntry>();
            _memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);
            cacheEntryMock.Setup(x => x.Value).Returns(apiMock);

            StocksController controller = new StocksController(_tradingOptions, _finnhubService, _stocksService, _memoryCache);

            IActionResult result = await controller.Explore("AAPL");
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<StockViewModel>>(viewResult.Model);
            Assert.NotNull(viewResult.ViewData["SelectedStock"]);
        }

        #endregion
    }
}
