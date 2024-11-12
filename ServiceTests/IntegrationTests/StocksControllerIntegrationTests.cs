using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests.IntegrationTests
{
    public class StocksControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public StocksControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        #region Explore

        [Fact]
        public async Task Explore_WithoutSymbol()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/Stocks/Explore");

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            Assert.NotNull(document.QuerySelectorAll(".stocks-item"));
        }

        [Fact]
        public async Task Explore_WithProperSymbol()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/Stocks/Explore/AAPL");

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            Assert.NotNull(document.QuerySelectorAll(".stocks-item"));
            Assert.NotNull(document.QuerySelector(".selected-stock-container"));
        }

        [Fact]
        public async Task Explore_WithInvalidSymbol()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/Stocks/Explore/dummytestsymbol");

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            Assert.NotNull(document.QuerySelectorAll(".error-item"));
            Assert.Null(document.QuerySelector(".selected-stock-container"));
        }

        #endregion
    }
}
