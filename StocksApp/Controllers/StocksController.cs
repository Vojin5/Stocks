using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
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

        public StocksController(IOptions<TradingOptions> options,IFinnhubService finnhubService,IStocksService stocksService)
        {
            _options = options.Value;
            _finnhubService = finnhubService;
            _stocksService = stocksService;
        }

        [Route("explore/{selectedStock?}")]
        public async Task<IActionResult> Explore(string? selectedStock)
        {
            if(_options == null || _options.Top25PopularStocks == null || _options.Top25PopularStocks.IsNullOrEmpty())
            {
                ViewBag.Errors = "Error with getting top stocks from options";
                return View();
            }
            List<Dictionary<string, string>>? stocks = await _finnhubService.GetStocks();
            if(stocks == null)
            {
                ViewBag.Errors = "Error while handling request to the finnhub,please try again";
                return View();
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
                ViewBag.SelectedStock = selectedStock;
            }
            return View(viewModels);
        }
    }
}
