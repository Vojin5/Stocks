using Entities;
using Entities.DTO;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests.IntegrationTests
{
    public class TradeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _customWebApplicationFactory;

        public TradeControllerIntegrationTests(CustomWebApplicationFactory webFactory)
        {
            _customWebApplicationFactory = webFactory;
            _httpClient = webFactory.CreateClient();

        }


        #region Index

        /// <summary>
        /// Index call with default symbol
        /// </summary>
        /// <returns>Index page without errors</returns>
        [Fact]
        public async Task Index_ToBeSucessful()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/trade/index");

            Assert.True(response.IsSuccessStatusCode);

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            Assert.NotNull(document.QuerySelector("#StockPrice"));
            Assert.NotNull(document.QuerySelector("#StockSymbol"));
            Assert.Null(document.QuerySelector(".error-item"));
        }

        /// <summary>
        /// Index call with real symbol
        /// </summary>
        /// <returns>Index page with information of stock provided by symbol</returns>
        [Fact]
        public async Task Index_WithStock()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/trade/index/AAPL");

            Assert.True(response.IsSuccessStatusCode);

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            Assert.NotNull(document.QuerySelector("#StockPrice"));
            Assert.NotNull(document.QuerySelector("#StockSymbol"));
            Assert.Null(document.QuerySelector(".error-item"));

            HtmlNode stockSymbol = document.QuerySelector("#StockSymbol");
            Assert.True(stockSymbol.InnerText == "AAPL");
        }

        /// <summary>
        /// When supplied with bad stock name should return view with errors and empty model
        /// </summary>
        /// <returns>View with Errors</returns>
        [Fact]
        public async Task Index_WithBadStock()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/trade/index/dummytextinvalidstock");

            Assert.True(response.IsSuccessStatusCode);

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            Assert.NotNull(document.QuerySelector(".error-container"));
            Assert.NotNull(document.QuerySelector(".error-item"));
        }

        #endregion

        #region BuyStock

        /// <summary>
        /// Succesful call to add new buyStocks with redirect to orders
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task BuyStocks_RedirectToOrders_Success()
        {

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("StockSymbol", "AAPL"),
                new KeyValuePair<string, string>("StockName", "Apple"),
                new KeyValuePair<string, string>("DateAndTimeOfOrder", DateTime.Now.ToString("o")),
                new KeyValuePair<string, string>("Quantity", "100"),
                new KeyValuePair<string, string>("Price", "100")
            };
            var content = new FormUrlEncodedContent(formData);

            HttpResponseMessage response = await _httpClient.PostAsync("/Trade/buyOrder", content);

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            Assert.True(document.QuerySelector("title").InnerText == "Orders");
            Assert.True(document.QuerySelector(".error-item") == null);
        }

        /// <summary>
        /// when supplied with some validation errors it returns index with error list
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task BuyStocks_RedirectToIndexWithErrors_IndexWithErrors()
        {

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("StockSymbol", "AAPL"),
                new KeyValuePair<string, string>("StockName", "Apple"),
                new KeyValuePair<string, string>("DateAndTimeOfOrder", DateTime.Now.ToString("o")),
                new KeyValuePair<string, string>("Quantity", "10000001"),
                new KeyValuePair<string, string>("Price", "100")
            };
            var content = new FormUrlEncodedContent(formData);

            HttpResponseMessage response = await _httpClient.PostAsync("/Trade/buyOrder", content);

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            Assert.True(document.QuerySelector("title").InnerText == "Stocks");
            Assert.True(document.QuerySelector(".error-item") != null);
        }

        #endregion

        #region SellStock

        /// <summary>
        /// Succesful call to add new buyStocks with redirect to orders
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SellStocks_RedirectToOrders_Success()
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("StockSymbol", "AAPL"),
                new KeyValuePair<string, string>("StockName", "Apple"),
                new KeyValuePair<string, string>("DateAndTimeOfOrder", DateTime.Now.ToString("o")),
                new KeyValuePair<string, string>("Quantity", "100"),
                new KeyValuePair<string, string>("Price", "100")
            };
            var content = new FormUrlEncodedContent(formData);

            HttpResponseMessage response = await _httpClient.PostAsync("/Trade/sellOrder", content);

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            Assert.True(document.QuerySelector("title").InnerText == "Orders");
            Assert.True(document.QuerySelector(".error-item") == null);
        }

        [Fact]
        public async Task SellStocks_RedirectToIndexWithErrors_IndexWithErrors()
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("StockSymbol", "AAPL"),
                new KeyValuePair<string, string>("StockName", "Apple"),
                new KeyValuePair<string, string>("DateAndTimeOfOrder", DateTime.Now.ToString("o")),
                new KeyValuePair<string, string>("Quantity", "10000001"),
                new KeyValuePair<string, string>("Price", "100")
            };
            var content = new FormUrlEncodedContent(formData);

            HttpResponseMessage response = await _httpClient.PostAsync("/Trade/sellOrder", content);

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            Assert.True(document.QuerySelector("title").InnerText == "Stocks");
            Assert.True(document.QuerySelector(".error-item") != null);
        }

        #endregion

        #region Orders

        [Fact]
        public async Task Orders_NonEmptyLists()
        {
            var response = await _httpClient.GetAsync("/Trade/orders");

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            Assert.True(document.QuerySelectorAll(".buyOrders-container .order").Count() != 0);
            Assert.True(document.QuerySelectorAll(".sellOrders-container .order").Count() != 0);
        }


        #endregion

        #region OrdersPDF

        [Fact]
        public async Task OrdersPDF_Get()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/Trade/OrdersPDF");
            Assert.True(response.IsSuccessStatusCode);

            Assert.Contains("application/pdf", response.Content.Headers.ContentType?.ToString());
        }

        #endregion
    }
}
