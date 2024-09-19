using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IFinnhubService? _finnhubService;
        public FinnhubServiceTests()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddUserSecrets<FinnhubServiceTests>();
            IConfiguration config = configurationBuilder.Build();

            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<HttpClient>(new HttpClient());
            services.AddTransient<IFinnhubService, FinnhubService>();

            var serviceProvider = services.BuildServiceProvider();

            _finnhubService = serviceProvider.GetService<IFinnhubService>();
        }

        #region GetCompanyProfile

        //!!Provide api key in the ServiceTest user-secrets

        //Valid stockSymbol and expect data in return
        [Fact]
        public async void GetCompanyProfile_ValidSymbol()
        {
            if (_finnhubService == null)
                throw new Exception("Finnhub service cannot be initialized");

            Dictionary<string,object>? result = await _finnhubService.GetCompanyProfile("MSFT");
            Assert.NotNull(result);
        }

        //Invalid StockSymbol provided and expect an InvalidOperationException
        [Fact]
        public async void GetCompanyProfile_InvalidSymbol()
        {
            if (_finnhubService == null)
                throw new Exception("Finnhub service cannot be initialized");

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                Dictionary<string, object>? result = 
                await _finnhubService.GetCompanyProfile("DummyText");
            });
        }

        #endregion

        #region GetStockPriceQuote

        //!!Provide api key in the ServiceTest user-secrets

        //Valid stockSymbol and expect data in return
        [Fact]
        public async void GetStockPrice_ValidSymbol()
        {
            if (_finnhubService == null)
                throw new Exception("Finnhub service cannot be initialized");

            Dictionary<string, object>? result = await _finnhubService.GetStockPriceQuote("MSFT");
            Assert.NotNull(result);
        }

        //Invalid StockSymbol provided and expect an InvalidOperationException
        [Fact]
        public async void GetStockPrice_InvalidSymbol()
        {
            if (_finnhubService == null)
                throw new Exception("Finnhub service cannot be initialized");

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                Dictionary<string, object>? result =
                await _finnhubService.GetStockPriceQuote("DummyText");
            });
        }

        #endregion
    }
}
