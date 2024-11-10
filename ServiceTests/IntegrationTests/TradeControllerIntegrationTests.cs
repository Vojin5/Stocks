using Entities;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }


        #endregion
    }
}
