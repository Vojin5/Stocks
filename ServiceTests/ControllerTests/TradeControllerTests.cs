﻿using Microsoft.Extensions.Options;
using Moq;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StocksApp.ConfiguraitonOptions;
using StocksApp.Controllers;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Entities.ViewModels;
using Entities.DTO;
using Microsoft.AspNetCore.Http;
using Entities;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ServiceTests.ControllerTests
{
    public class TradeControllerTests
    {
        private readonly IStocksService _stocksService;
        private readonly IFinnhubService _finnhubService;
        private readonly IOptions<TradingOptions> _tradingOptions;

        private readonly Mock<IStocksService> _stocksServiceMock;
        private readonly Mock<IFinnhubService> _finnhubServiceMock;
        private readonly Mock<IOptions<TradingOptions>> _optionsMock;

        private readonly IFixture _fixture;

        public TradeControllerTests()
        {
            _finnhubServiceMock = new Mock<IFinnhubService>();
            _stocksServiceMock = new Mock<IStocksService>();
            _optionsMock = new Mock<IOptions<TradingOptions>>();

            _stocksService = _stocksServiceMock.Object;
            _finnhubService = _finnhubServiceMock.Object;
            _tradingOptions = _optionsMock.Object;

            _fixture = new Fixture();

        }
        //Index Method used for root to display default trade option
        //Could display errors that encounter on buy or sell
        //Could display other stocks if passed another symbol on route
        #region Index

        /// <summary>
        /// Checks : view is view result, no errors, view name is Index, model is StockTrade
        /// </summary>
        /// <returns>Success</returns>
        [Fact]
        public async Task Index_ValidInputNoErrors_ToGetIndexView()
        {
            //options provided
            TradingOptions tradingOptionsMock = new TradingOptions();
            tradingOptionsMock.DefaultStockSymbol = "MSFT";
            _optionsMock.Setup(x => x.Value).Returns(tradingOptionsMock);

            //mock data for company profile
            Dictionary<string, object>? mockResultCompany = new Dictionary<string, object>()
            {
                {"name","Microsoft Corp" },
                {"ticker","MSFT" }
            };
            _finnhubServiceMock.Setup(x => x.GetCompanyProfile(It.IsAny<string>())).ReturnsAsync(mockResultCompany);

            //mock data for stock 
            Dictionary<string, object> mockResultStock = new Dictionary<string, object>()
            {
                {"c",123.2 }
            };
            _finnhubServiceMock.Setup(x => x.GetStockPriceQuote(It.IsAny<string>())).ReturnsAsync(mockResultStock);

            //temp data mock
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            //setting tempData
            controller.TempData = tempData;

            IActionResult result = await controller.Index("AAPL");

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<StockTrade>(viewResult.ViewData.Model);
            Assert.True(viewResult.ViewName == "Index");
            Assert.True(viewResult.TempData["Errors"] == null);
        }

        [Fact]
        public async Task Index_ValidInputWithErrors_ToGetIndexView()
        {
            TradingOptions tradingOptionsMock = new TradingOptions();
            tradingOptionsMock.DefaultStockSymbol = "MSFT";
            _optionsMock.Setup(x => x.Value).Returns(tradingOptionsMock);

            Dictionary<string, object>? mockResultCompany = new Dictionary<string, object>()
            {
                {"name","Microsoft Corp" },
                {"ticker","MSFT" }
            };
            _finnhubServiceMock.Setup(x => x.GetCompanyProfile(It.IsAny<string>())).ReturnsAsync(mockResultCompany);
            Dictionary<string, object> mockResultStock = new Dictionary<string, object>()
            {
                {"c",123.2 }
            };
            _finnhubServiceMock.Setup(x => x.GetStockPriceQuote(It.IsAny<string>())).ReturnsAsync(mockResultStock);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            tempData["Errors"] = new string[] { "Sample Error" };

            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.Index(null);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<StockTrade>(viewResult.ViewData.Model);
            Assert.True(viewResult.ViewName == "Index");
            Assert.False(viewResult.TempData["Errors"] == null);
            Assert.True(viewResult.ViewData["Errors"] != null);
        }


        /// <summary>
        /// No Trading options with default symbol provided
        /// </summary>
        /// <returns>ArgumentNullException</returns>
        [Fact]
        public async Task Index_NoTradingOptions_ArgumentNullException()
        {
            /*TradingOptions tradingOptionsMock = new TradingOptions();
            tradingOptionsMock.DefaultStockSymbol = "MSFT";
            _optionsMock.Setup(x => x.Value).Returns(tradingOptionsMock);*/

            Dictionary<string, object>? mockResultCompany = new Dictionary<string, object>()
            {
                {"name","Microsoft Corp" },
                {"ticker","MSFT" }
            };
            _finnhubServiceMock.Setup(x => x.GetCompanyProfile(It.IsAny<string>())).ReturnsAsync(mockResultCompany);
            Dictionary<string, object> mockResultStock = new Dictionary<string, object>()
            {
                {"c",123.2 }
            };
            _finnhubServiceMock.Setup(x => x.GetStockPriceQuote(It.IsAny<string>())).ReturnsAsync(mockResultStock);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await controller.Index(null);
            });
        }

        /// <summary>
        /// If api returns no result there will be error message on view.
        /// </summary>
        /// <returns>View with errors</returns>
        [Fact]
        public async Task Index_NoApiResponses_ToGetIndexViewWithErrors()
        {
            TradingOptions tradingOptionsMock = new TradingOptions();
            tradingOptionsMock.DefaultStockSymbol = "MSFT";
            _optionsMock.Setup(x => x.Value).Returns(tradingOptionsMock);

            /*Dictionary<string, object>? mockResultCompany = new Dictionary<string, object>()
            {
                {"name","Microsoft Corp" },
                {"ticker","MSFT" }
            };
            _finnhubServiceMock.Setup(x => x.GetCompanyProfile(It.IsAny<string>())).ReturnsAsync(mockResultCompany);
            Dictionary<string, object> mockResultStock = new Dictionary<string, object>()
            {
                {"c",123.2 }
            };
            _finnhubServiceMock.Setup(x => x.GetStockPriceQuote(It.IsAny<string>())).ReturnsAsync(mockResultStock);*/
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.Index(null);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewName == "Index");
            Assert.False(viewResult.ViewData["Errors"] == null);
        }

        /// <summary>
        /// If api returns no result for bad symbol provided it should display errors
        /// </summary>
        /// <returns>View with errors</returns>
        [Fact]
        public async Task Index_BadStockSymbol_ToGetIndexViewWithErrors()
        {
            TradingOptions tradingOptionsMock = new TradingOptions();
            tradingOptionsMock.DefaultStockSymbol = "MSFT";
            _optionsMock.Setup(x => x.Value).Returns(tradingOptionsMock);

            /*Dictionary<string, object>? mockResultCompany = new Dictionary<string, object>()
            {
                {"name","Microsoft Corp" },
                {"ticker","MSFT" }
            };
            _finnhubServiceMock.Setup(x => x.GetCompanyProfile(It.IsAny<string>())).ReturnsAsync(mockResultCompany);
            Dictionary<string, object> mockResultStock = new Dictionary<string, object>()
            {
                {"c",123.2 }
            };
            _finnhubServiceMock.Setup(x => x.GetStockPriceQuote(It.IsAny<string>())).ReturnsAsync(mockResultStock);*/
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;


            IActionResult result = await controller.Index("dummybadsymbol");

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewName == "Index");
            Assert.False(viewResult.ViewData["Errors"] == null);
        }

        #endregion
        //Performs the insertion into database a new BuyOrder
        //Receives BuyOrderRequest and redirects to Orders Page
        //Redirects to Index with errors on failure
        #region BuyOrder

        /// <summary>
        /// Creates buyOrderRequest and passes it to controler for check
        /// </summary>
        /// <returns>Redirect to Orders action method</returns>
        [Fact]
        public async Task BuyOrder_ValidInput_ToBeRedirectToOrders()
        {
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .Create();
            BuyOrder buyOrder = request.ToBuyOrder();
            buyOrder.BuyOrderID = Guid.NewGuid();
            BuyOrderResponse response = buyOrder.ToBuyOrderResponse();

            _stocksServiceMock.Setup(x => x.CreateBuyOrder(It.IsAny<BuyOrderRequest>())).
                ReturnsAsync(response);

            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);

            IActionResult result = await controller.BuyOrder(request);
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.True(redirectResult.ActionName == "Orders"
                && redirectResult.ControllerName == "Trade");
        }

        /// <summary>
        /// Creates buyOrderRequest that does not meet requirements of Model validation
        /// </summary>
        /// <returns>Redirect to Index action method with errors of validations</returns>
        [Fact]
        public async Task BuyOrder_InvalidInput_ToBeRedirectToIndex()
        {
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100001)
                .With(x => x.Price, 100)
                .Create();

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.BuyOrder(request);
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.True(redirectResult.ActionName == "Index");
        }

        /// <summary>
        /// If there are any problems with database it should return null and display
        /// errors on index
        /// </summary>
        /// <returns>Redirect to Index with errors</returns>
        [Fact]
        public async Task BuyOrder_ErrorsWithDatabase_ToBeRedirectToIndex()
        {
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .Create();

            _stocksServiceMock.Setup(x => x.CreateBuyOrder(It.IsAny<BuyOrderRequest>()))
                .ReturnsAsync(null as BuyOrderResponse);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.BuyOrder(request);
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.True(redirectResult.ActionName == "Index");
            
        }

        #endregion
        //Performs the insertion into database a new SellOrder
        //Receives SellOrderRequest and redirects to Order Page
        //Redirects to Index with errors on failure
        #region SellOrder

        /// <summary>
        /// Creates buyOrderRequest and passes it to controler for check
        /// </summary>
        /// <returns>Redirect to Orders action method</returns>
        [Fact]
        public async Task SellOrder_ValidInput_ToBeRedirectToOrders()
        {
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .Create();

            SellOrder sellOrder = request.ToSellOrder();
            sellOrder.SellOrderID = Guid.NewGuid();
            SellOrderResponse response = sellOrder.ToSellOrderResponse();
            _stocksServiceMock.Setup(x => x.CreateSellOrder(It.IsAny<SellOrderRequest>())).ReturnsAsync(response);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.SellOrder(request);
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.True(redirectResult.ActionName == "Orders"
                && redirectResult.ControllerName == "Trade");

        }

        /// <summary>
        /// Creates buyOrderRequest that does not meet requirements of Model validation
        /// </summary>
        /// <returns>Redirect to Index action method with errors of validations</returns>
        [Fact]
        public async Task SellOrder_InvalidInput_ToBeRedirectToIndex()
        {
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100001)
                .With(x => x.Price, 100)
                .Create();

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.SellOrder(request);
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.True(redirectResult.ActionName == "Index");
            
        }

        /// <summary>
        /// If there are any problems with database it should return null
        /// and redirect to index with errors
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SellOrder_ErrorsWithDatabase_ToBeRedirectToIndex()
        {
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .Create();

            _stocksServiceMock.Setup(x => x.CreateSellOrder(It.IsAny<SellOrderRequest>())).ReturnsAsync(null as SellOrderResponse);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.SellOrder(request);
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.True(redirectResult.ActionName == "Index");
        }

        #endregion
        //Returns all Orders from database with view model OrdersViewModel
        #region Orders
        /// <summary>
        /// Getting ViewModel with null properties
        /// </summary>
        /// <returns>View</returns>
        [Fact]
        public async Task Orders_EmptyList_ToReturnView()
        {
            _stocksServiceMock.Setup(x => x.GetSellOrders()).ReturnsAsync(new List<SellOrderResponse>());
            _stocksServiceMock.Setup(x => x.GetBuyOrders()).ReturnsAsync(new List<BuyOrderResponse>());
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;
            IActionResult result = await controller.Orders();
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            OrdersViewModel viewModel = Assert.IsAssignableFrom<OrdersViewModel>(viewResult.Model);
        }

        /// <summary>
        /// Getting view with model that has lists
        /// </summary>
        /// <returns>View</returns>
        [Fact]
        public async Task Orders_List_ToReturnView()
        {
            List<BuyOrderResponse> buyOrdersMock = _fixture.Build<BuyOrderResponse>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .With(x => x.TradeAmount)
                .CreateMany(5)
                .ToList();

            List<SellOrderResponse> sellOrdersMock = _fixture.Build<SellOrderResponse>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .With(x => x.TradeAmount)
                .CreateMany(5)
                .ToList();

            _stocksServiceMock.Setup(x => x.GetBuyOrders()).ReturnsAsync(buyOrdersMock);
            _stocksServiceMock.Setup(x => x.GetSellOrders()).ReturnsAsync(sellOrdersMock);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.Orders();
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            OrdersViewModel viewModel = Assert.IsAssignableFrom<OrdersViewModel>(viewResult.Model);
            Assert.NotNull(viewModel.SellOrders);
            Assert.NotNull(viewModel.BuyOrders);
        }

        [Fact]
        public async Task Orders_WithErrors_ToReturnView()
        {
            List<BuyOrderResponse> buyOrdersMock = _fixture.Build<BuyOrderResponse>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .With(x => x.TradeAmount)
                .CreateMany(5)
                .ToList();

            List<SellOrderResponse> sellOrdersMock = _fixture.Build<SellOrderResponse>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .With(x => x.TradeAmount)
                .CreateMany(5)
                .ToList();

            _stocksServiceMock.Setup(x => x.GetBuyOrders()).ReturnsAsync(buyOrdersMock);
            _stocksServiceMock.Setup(x => x.GetSellOrders()).ReturnsAsync(sellOrdersMock);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            tempData["Errors"] = new string[] { "Sample Error" };
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.Orders();
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            OrdersViewModel viewModel = Assert.IsAssignableFrom<OrdersViewModel>(viewResult.Model);
            Assert.NotNull(viewModel.SellOrders);
            Assert.NotNull(viewModel.BuyOrders);
            Assert.True(viewResult.ViewData["Errors"] != null);
            Assert.True(viewResult.TempData["Errors"] != null);
        }

        [Fact]
        public async Task Orders_ErrorsWithDatabase_ToReturnView()
        {
            _stocksServiceMock.Setup(x => x.GetSellOrders()).ReturnsAsync(null as List<SellOrderResponse>);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;
            IActionResult result = await controller.Orders();
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            OrdersViewModel viewModel = Assert.IsAssignableFrom<OrdersViewModel>(viewResult.Model);
            Assert.True(viewResult.ViewData["Errors"] != null);
        }

        #endregion
        //Returns all Orders from database in form of PDF
        #region OrdersPDF

        /// <summary>
        /// Getting ViewAsPdf result with empty data from database
        /// </summary>
        /// <returns>ViewAsPdf</returns>
        [Fact]
        public async Task OrdersPDF_EmptyList_ToReturnView()
        {
            _stocksServiceMock.Setup(x => x.GetSellOrders()).ReturnsAsync(new List<SellOrderResponse>());
            _stocksServiceMock.Setup(x => x.GetBuyOrders()).ReturnsAsync(new List<BuyOrderResponse>());

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.OrdersPDF();
            ViewAsPdf viewResult = Assert.IsType<ViewAsPdf>(result);
            OrdersViewModel viewModel = Assert.IsAssignableFrom<OrdersViewModel>(viewResult.Model);
            Assert.NotNull(viewModel.SellOrders);
            Assert.NotNull(viewModel.BuyOrders);
        }
        /// <summary>
        /// Getting ViewAsPdf with data from database 
        /// </summary>
        /// <returns>ViewAsPdf</returns>
        [Fact]
        public async Task OrdersPDF_List_ToReturnView()
        {
            List<BuyOrderResponse> buyOrdersMock = _fixture.Build<BuyOrderResponse>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .With(x => x.TradeAmount)
                .CreateMany(5)
                .ToList();

            List<SellOrderResponse> sellOrdersMock = _fixture.Build<SellOrderResponse>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .With(x => x.TradeAmount)
                .CreateMany(5)
                .ToList();

            _stocksServiceMock.Setup(x => x.GetBuyOrders()).ReturnsAsync(buyOrdersMock);
            _stocksServiceMock.Setup(x => x.GetSellOrders()).ReturnsAsync(sellOrdersMock);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.OrdersPDF();
            ViewAsPdf viewResult = Assert.IsType<ViewAsPdf>(result);
            OrdersViewModel viewModel = Assert.IsAssignableFrom<OrdersViewModel>(viewResult.Model);
            Assert.NotNull(viewModel.SellOrders);
            Assert.NotNull(viewModel.BuyOrders);
        }

        [Fact]
        public async Task OrdersPDF_DatabaseError_ToReturnView()
        {
            _stocksServiceMock.Setup(x => x.GetSellOrders()).ReturnsAsync(null as List<SellOrderResponse>);
            _stocksServiceMock.Setup(x => x.GetBuyOrders()).ReturnsAsync(null as List<BuyOrderResponse>);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            TradeController controller = new TradeController(_tradingOptions, _finnhubService, _stocksService);
            controller.TempData = tempData;

            IActionResult result = await controller.OrdersPDF();
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.True(redirectResult.ActionName == "Orders");
        }

        #endregion


    }
}
