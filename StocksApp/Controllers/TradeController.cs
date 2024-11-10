using Entities;
using Entities.DTO;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public async Task<IActionResult> Index(string? stockSymbol)
        {
            //Errors from other methods
            string[]? errors = (string[]?) TempData["Errors"];
            if(errors != null)
            {
                ViewBag.Errors = errors;
            }

            //trading options
            if(_tradingOptions == null || _tradingOptions.DefaultStockSymbol == null)
                throw new ArgumentNullException(nameof(TradingOptions));

            //provided stock symbol or default
            if (stockSymbol.IsNullOrEmpty() || stockSymbol == null)
                stockSymbol = _tradingOptions.DefaultStockSymbol;

            //company data based on provided stock symbol
            Dictionary<string, object>? companyProfileDictionary =
               await _finnhubService.GetCompanyProfile(stockSymbol);

            //stock data based on stock symbol
            Dictionary<string, object>? stockProfileDictionary =
                await _finnhubService.GetStockPriceQuote(stockSymbol);

            //internal api errors
            if (companyProfileDictionary == null || stockProfileDictionary == null)
            {
                ViewBag.Errors = new List<string>() { "There was an error while fetching stocks data" };
                return View("Index", new StockTrade());
            }

            StockTrade stockTrade = new StockTrade()
            {
                StockName = companyProfileDictionary["name"].ToString(),
                StockSymbol = companyProfileDictionary["ticker"].ToString(),
                Price = Convert.ToDouble(stockProfileDictionary["c"].ToString()),
                Quantity = _tradingOptions.DefaultOrderQuantity
            };
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
                TempData["Errors"] = errors;
                return RedirectToAction("Index");
            }
            else
            {
                BuyOrderResponse? response = await _stocksService.CreateBuyOrder(buyOrderRequest);
                if (response == null)
                {
                    List<string?> errors = 
                        validationResults.Select(x => x.ErrorMessage).ToList();
                    TempData["Errors"] = errors;
                    return RedirectToAction("Index");
                }
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

            if (isValid)
            {
                List<string?> errors = validationResults.Select(x => x.ErrorMessage).ToList();
                TempData["Errors"] = errors;
                return RedirectToAction("Index");
            }
            else
            {
                SellOrderResponse? response = await _stocksService.CreateSellOrder(sellOrderRequest);
                if(response == null)
                {
                    List<string> errors = new List<string> { "There was an error with create request please try again" };
                    TempData["Errors"] = errors;
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Orders", "Trade");
            }
        }

        [Route("orders")]
        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            string[]? errors = (string[]?) TempData["Errors"];
            if (errors != null)
                ViewData["Errors"] = errors;

            List<BuyOrderResponse>? buyOrders = await _stocksService.GetBuyOrders();
            List<SellOrderResponse>? sellOrders = await _stocksService.GetSellOrders();
            if (buyOrders == null || sellOrders == null)
            {
                ViewBag.Errors = "There were problems with getting orders, please try again";
                OrdersViewModel emptyOrders = new OrdersViewModel()
                {
                    BuyOrders = new List<BuyOrderResponse>(),
                    SellOrders = new List<SellOrderResponse>()
                };
                return View(emptyOrders);
            }
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
            List<BuyOrderResponse>? buyOrders = await _stocksService.GetBuyOrders();
            List<SellOrderResponse>? sellOrders = await _stocksService.GetSellOrders();
            if(buyOrders == null || sellOrders == null)
            {
                List<string> errors = new List<string>() { "Error while getting orders from database, please try again later(PDF)" };
                TempData["Errors"] = errors;
                return RedirectToAction("Orders");
            }
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
