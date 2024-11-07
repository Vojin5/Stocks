using Entities;
using Entities.DTO;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rotativa.AspNetCore;
using ServiceContracts;
using StocksApp.ConfiguraitonOptions;
using System.ComponentModel.DataAnnotations;

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

        [Route("index/{stockSymbol?}")]
        [Route("/")]
        public async Task<IActionResult> Index(List<string>? Errors,
            string? stockSymbol)
        {
            if(_tradingOptions == null || _tradingOptions.DefaultStockSymbol == null)
                throw new ArgumentNullException(nameof(TradingOptions));

            if (stockSymbol.IsNullOrEmpty() || stockSymbol == null)
                stockSymbol = _tradingOptions.DefaultStockSymbol;

            Dictionary<string, object>? companyProfileDictionary =
               await _finnhubService.GetCompanyProfile(stockSymbol);

            Dictionary<string, object>? stockProfileDictionary =
                await _finnhubService.GetStockPriceQuote(stockSymbol);

            if (companyProfileDictionary == null || stockProfileDictionary == null)
            {
                ViewBag.Errors = new List<string>() { "There was an error while fetching stocks data" };
                return View("Index", null);
            }

            StockTrade stockTrade = new StockTrade()
            {
                StockName = companyProfileDictionary["name"].ToString(),
                StockSymbol = companyProfileDictionary["ticker"].ToString(),
                Price = Convert.ToDouble(stockProfileDictionary["c"].ToString()),
                Quantity = _tradingOptions.DefaultOrderQuantity
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
            var validationContext = new ValidationContext(buyOrderRequest);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(buyOrderRequest, validationContext, validationResults, true);

            if (!isValid)
            {
                List<string?> errors = validationResults.Select(x =>x.ErrorMessage).ToList();
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
            var validationContext = new ValidationContext(sellOrderRequest);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(sellOrderRequest, validationContext, validationResults, true);

            if (!isValid)
            {

                List<string?> errors = validationResults.Select(x => x.ErrorMessage).ToList();
                return RedirectToAction("Index", new { Errors = errors });
            }
            else
            {
                await _stocksService.CreateSellOrder(sellOrderRequest);
                return RedirectToAction("Orders", "Trade");
            }
        }

        [Route("orders")]
        [HttpGet]
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

        [Route("ordersPDF")]
        [HttpGet]
        public async Task<IActionResult> OrdersPDF()
        {
            List<BuyOrderResponse> buyOrders = await _stocksService.GetBuyOrders();
            List<SellOrderResponse> sellOrders = await _stocksService.GetSellOrders();
            OrdersViewModel orders = new OrdersViewModel()
            {
                BuyOrders = buyOrders,
                SellOrders = sellOrders
            };
            return new ViewAsPdf("OrdersPDF", orders, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins()
                {
                    Top = 20,
                    Bottom = 20,
                    Right = 20,
                    Left = 20
                },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }
    }
}
