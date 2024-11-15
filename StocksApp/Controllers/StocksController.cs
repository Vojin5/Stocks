using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServiceContracts;
using StocksApp.ConfiguraitonOptions;

namespace StocksApp.Controllers
{
    [Route("[controller]")]
    public class StocksController : Controller
    {
        private readonly TradingOptions _options;
        private readonly IFinnhubService _finnhubService;
        private readonly IStocksService _stocksService;

        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;

        public StocksController(IOptions<TradingOptions> options,IFinnhubService finnhubService,IStocksService stocksService,IMemoryCache memoryCache,ILogger<StocksController> logger)
        {
            _options = options.Value;
            _finnhubService = finnhubService;
            _stocksService = stocksService;
            _cache = memoryCache;
            _logger = logger;

            _logger.LogInformation("Stocks Controller started");
        }

        private async Task<List<Dictionary<string,string>>?> getStocksCached()
        {
            _logger.LogInformation("Accessing cache for stocks data");
            return await _cache.GetOrCreateAsync("GetStocks", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _finnhubService.GetStocks();
            });
        }

        [Route("explore/{selectedStock?}")]
        public async Task<IActionResult> Explore(string? selectedStock)
        {
            _logger.LogInformation("Stocks Controller : Explore");

            if(_options == null || _options.Top25PopularStocks == null || _options.Top25PopularStocks.IsNullOrEmpty())
            {
                ViewBag.Errors = "Error with getting top stocks from options";
                _logger.LogError("Stocks Controller : Explore : Error : Error while getting top stocks from options");
                return View(new List<StockViewModel>());
            }

            List<Dictionary<string, string>>? stocks = await this.getStocksCached();
            if(stocks == null)
            {
                ViewBag.Errors = "Error while handling request to the finnhub,please try again";
                _logger.LogError("Stocks Controller : Explore : Error : Error while handling request to the finnhub");
                return View(new List<StockViewModel>());
            }
            List<string> popularStocks = _options.Top25PopularStocks;
            stocks = stocks.Where(x => popularStocks.Contains(x["symbol"])).ToList();
            List<StockViewModel> viewModels = new List<StockViewModel>();
            foreach(var stock in stocks)
            {
                viewModels.Add(new StockViewModel()
                {
                    StockName = stock["description"],
                    StockSymbol = stock["symbol"]
                });
            }
            if(!string.IsNullOrEmpty(selectedStock))
            {
                _logger.LogInformation($"Selected stock : {selectedStock}");
                ViewBag.SelectedStock = selectedStock;
            }
            _logger.LogDebug($"Stocks Controller : Explore view model : {viewModels.ToString()}");
            return View(viewModels);
        }
    }
}
