using Entities;
using Entities.DTO;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceContracts;
using StocksApp.ConfiguraitonOptions;

namespace StocksApp.Controllers
{
    [Route("[controller]")]
    public class TradeController : Controller
    {
        private readonly TradingOptions _tradingOptions;
        private readonly IFinnhubService _finnhubService;
        private readonly IStocksService _stocksService;

        public TradeController(IOptions<TradingOptions> tradingOptions,
            IFinnhubService finnhubService,
            IStocksService stocksService)
        {
            _tradingOptions = tradingOptions.Value;
            _finnhubService = finnhubService;
            _stocksService = stocksService;
        }

        [Route("index")]
        [Route("/")]
        public async Task<IActionResult> Index(List<string>? Errors)
        {
            if(_tradingOptions == null || _tradingOptions.DefaultStockSymbol == null)
                throw new ArgumentNullException(nameof(TradingOptions));

            Dictionary<string, object>? companyProfileDictionary =
               await _finnhubService.GetCompanyProfile(_tradingOptions.DefaultStockSymbol);

            Dictionary<string, object>? stockProfileDictionary =
                await _finnhubService.GetStockPriceQuote(_tradingOptions.DefaultStockSymbol);

            if (companyProfileDictionary == null || stockProfileDictionary == null)
                throw new Exception("Error while handling Finnhub request,dictionary is null");

            StockTrade stockTrade = new StockTrade()
            {
                StockName = companyProfileDictionary["name"].ToString(),
                StockSymbol = companyProfileDictionary["ticker"].ToString(),
                Price = Convert.ToDouble(stockProfileDictionary["c"].ToString())
            };
            if (Errors != null)
            {
                ViewBag.Errors = Errors;
            }
            return View("Index",stockTrade);
        }


        [Route("buyOrder")]
        [HttpPost]
        public async Task<IActionResult> BuyOrder(BuyOrderRequest buyOrderRequest)
        {
            buyOrderRequest.DateAndTimeOfOrder = DateTime.Now;
            ModelState.Clear();
            TryValidateModel(buyOrderRequest);
            if(!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList();
                return RedirectToAction("Index", new {Errors = errors});
            }
            else
            {
                await _stocksService.CreateBuyOrder(buyOrderRequest);
                return RedirectToAction("Orders", "Trade");
            }
        }

        [Route("sellOrder")]
        [HttpPost]
        public async Task<IActionResult> SellOrder(SellOrderRequest sellOrderRequest)
        {
            sellOrderRequest.DateAndTimeOfOrder = DateTime.Now;
            ModelState.Clear();
            TryValidateModel(sellOrderRequest);
            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList();
                return RedirectToAction("Index", new { Errors = errors });
            }
            else
            {
                await _stocksService.CreateSellOrder(sellOrderRequest);
                return RedirectToAction("Orders", "Trade");
            }
        }

        [Route("orders")]
        public async Task<IActionResult> Orders()
        {
            List<BuyOrderResponse> buyOrders = await _stocksService.GetBuyOrders();
            List<SellOrderResponse> sellOrders = await _stocksService.GetSellOrders();
            OrdersViewModel orders = new OrdersViewModel()
            {
                BuyOrders = buyOrders,
                SellOrders = sellOrders
            };
            return View(orders);
        }
    }
}
