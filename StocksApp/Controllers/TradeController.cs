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
        private readonly ILogger<TradeController> _logger;

        public TradeController(IOptions<TradingOptions> tradingOptions,
            IFinnhubService finnhubService,
            IStocksService stocksService,
            ILogger<TradeController> logger)
        {
            _tradingOptions = tradingOptions.Value;
            _finnhubService = finnhubService;
            _stocksService = stocksService;
            _logger = logger;
        }

        [Route("index/{stockSymbol?}")]
        [Route("/")]
        public async Task<IActionResult> Index(string? stockSymbol)
        {
            _logger.LogInformation($"Trade Controller : Index with symbol : {stockSymbol}");
            _logger.LogDebug($"Trade Controller : Index : stockSymbol argument : {stockSymbol}");

            //Errors from other methods
            string[]? errors = (string[]?) TempData["Errors"];
            if(errors != null)
            {
                _logger.LogError($"Trade Controller : Index : Received Errors : {errors}");
                ViewBag.Errors = errors;
            }

            //trading options
            if(_tradingOptions == null || _tradingOptions.DefaultStockSymbol == null)
            {
                _logger.LogCritical("Trade Controller : Critical : No trading options provided");
                throw new ArgumentNullException(nameof(TradingOptions));
            }

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
                _logger.LogError("Trade Controller : Index : There were errors while fetching data");
                ViewBag.Errors = new List<string>() { "There was an error while fetching stocks data, returning index with errors" };
                return View("Index", new StockTrade());
            }

            StockTrade stockTrade = new StockTrade()
            {
                StockName = companyProfileDictionary["name"].ToString(),
                StockSymbol = companyProfileDictionary["ticker"].ToString(),
                Price = Convert.ToDouble(stockProfileDictionary["c"].ToString()),
                Quantity = _tradingOptions.DefaultOrderQuantity
            };

            _logger.LogDebug($"Trade Controller : Index returns view model : {stockTrade.ToString()}");
            _logger.LogInformation("Trade Controller : Index : returning Index View...");
            return View("Index",stockTrade);
        }


        [Route("buyOrder")]
        [HttpPost]
        public async Task<IActionResult> BuyOrder(BuyOrderRequest buyOrderRequest)
        {
            _logger.LogInformation("Trade Controller : BuyOrder");
            _logger.LogDebug($"Trade Controller : BuyOrder : request : {buyOrderRequest}");
            buyOrderRequest.DateAndTimeOfOrder = DateTime.Now;
            ModelState.Clear();
            var validationContext = new ValidationContext(buyOrderRequest);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(buyOrderRequest, validationContext, validationResults, true);

            if (!isValid)
            {
                List<string?> errors = validationResults.Select(x =>x.ErrorMessage).ToList();
                _logger.LogError($"Trade Controller : BuyOrder : Errors : {errors}");
                TempData["Errors"] = errors;
                _logger.LogInformation("Trade Controller : BuyOrder redirecting index with errors");
                return RedirectToAction("Index");
            }
            else
            {
                BuyOrderResponse? response = await _stocksService.CreateBuyOrder(buyOrderRequest);
                if (response == null)
                {
                    _logger.LogError($"Trade Controller : BuyOrder : Error : response is null, redirecting index with errors");
                    List<string?> errors = 
                        validationResults.Select(x => x.ErrorMessage).ToList();
                    TempData["Errors"] = errors;
                    return RedirectToAction("Index");
                }

                _logger.LogInformation("Trade Controller : BuyOrder : Redirecting to Orders");
                return RedirectToAction("Orders", "Trade");
            }
        }

        [Route("sellOrder")]
        [HttpPost]
        public async Task<IActionResult> SellOrder(SellOrderRequest sellOrderRequest)
        {
            _logger.LogInformation("Trade Controller : SellOrder");
            _logger.LogDebug($"Trade Controller : SellOrder : request : {sellOrderRequest}");

            sellOrderRequest.DateAndTimeOfOrder = DateTime.Now;
            ModelState.Clear();
            var validationContext = new ValidationContext(sellOrderRequest);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(sellOrderRequest, validationContext, validationResults, true);

            if (!isValid)
            {
                List<string?> errors = validationResults.Select(x => x.ErrorMessage).ToList();
                _logger.LogError($"Trade Controller : SellOrder : Errors : {errors}");
                TempData["Errors"] = errors;
                _logger.LogInformation("Redirecting to Index with errors");
                return RedirectToAction("Index");
            }
            else
            {
                SellOrderResponse? response = await _stocksService.CreateSellOrder(sellOrderRequest);
                if(response == null)
                {
                    List<string> errors = new List<string> { "There was an error with create request please try again" };
                    _logger.LogError($"Trade Controller : SellOrder : Errors {errors}");
                    TempData["Errors"] = errors;
                    _logger.LogInformation("Trade Controller : SellOrder redirecting to index with errors");
                    return RedirectToAction("Index");
                }

                _logger.LogInformation("Trade Controller : SellOrder redirecting to orders");
                return RedirectToAction("Orders", "Trade");
            }
        }

        [Route("orders")]
        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            _logger.LogInformation("Trade Controller : Orders");

            string[]? errors = (string[]?) TempData["Errors"];
            if (errors != null)
            {
                _logger.LogError($"Trade Controller : Orders received errors : {errors}");
                ViewData["Errors"] = errors;
            }

            List<BuyOrderResponse>? buyOrders = await _stocksService.GetBuyOrders();
            List<SellOrderResponse>? sellOrders = await _stocksService.GetSellOrders();
            if (buyOrders == null || sellOrders == null)
            {
                _logger.LogError("Trade Controller : Orders : Errors : There were problems with getting orders");
                ViewBag.Errors = "There were problems with getting orders, please try again";
                OrdersViewModel emptyOrders = new OrdersViewModel()
                {
                    BuyOrders = new List<BuyOrderResponse>(),
                    SellOrders = new List<SellOrderResponse>()
                };
                _logger.LogInformation("Trade Controller : Orders : Returning Empty orders with errors");
                return View(emptyOrders);
            }
            OrdersViewModel orders = new OrdersViewModel()
            {
                BuyOrders = buyOrders,
                SellOrders = sellOrders
            };

            _logger.LogDebug($"Trade Controllers : Orders : view model : {orders.ToString()}");
            _logger.LogInformation("Trade Controllers : Orders : returning orders");
            return View(orders);
        }

        [Route("ordersPDF")]
        [HttpGet]
        public async Task<IActionResult> OrdersPDF()
        {
            _logger.LogInformation("Trade Controller : OrdersPDF");

            List<BuyOrderResponse>? buyOrders = await _stocksService.GetBuyOrders();
            List<SellOrderResponse>? sellOrders = await _stocksService.GetSellOrders();
            if(buyOrders == null || sellOrders == null)
            {
                List<string> errors = new List<string>() { "Error while getting orders from database, please try again later(PDF)" };
                _logger.LogError($"Trade Controller : OrdersPDF : Errors : Errors while getting orders from database");
                TempData["Errors"] = errors;
                return RedirectToAction("Orders");
            }
            OrdersViewModel orders = new OrdersViewModel()
            {
                BuyOrders = buyOrders,
                SellOrders = sellOrders
            };
            _logger.LogDebug($"Trade Controller : OrdersPDF : viewmodel : {orders}");
            _logger.LogInformation($"Trade Controller : OrdersPDF returning pdf");
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
