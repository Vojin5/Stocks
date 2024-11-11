using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace StocksApp.ViewComponents
{
    public class SelectedStockViewComponent : ViewComponent
    {
        private readonly IFinnhubService _finnhubService;

        public SelectedStockViewComponent(IFinnhubService finnhubService)
        {
            _finnhubService = finnhubService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string stockSymbol)
        {
            Dictionary<string,object>? company = await _finnhubService.GetCompanyProfile(stockSymbol);
            Dictionary<string,object>? stockInfo = await _finnhubService.GetStockPriceQuote(stockSymbol);

            if (company == null || stockInfo == null)
            {
                Console.WriteLine("Error while handling selected stock request");
                ViewBag.Errors = "Error while handling selected stocks request";
                return View(null as SelectedStockViewComponent);
            }
            if (!company.ContainsKey("logo") || !company.ContainsKey("name") ||
                !company.ContainsKey("finnhubIndustry") || !company.ContainsKey("exchange"))
            {
                Console.WriteLine("Error while getting company data");
                ViewBag.Errors = "Error while handling selected stocks request";
                return View(null as SelectedStockViewComponent);
            }
            if (!stockInfo.ContainsKey("c"))
            {
                Console.WriteLine("Error while getting current price of stock");
                ViewBag.Errors = "Error while handling selected stocks request";
                return View(null as SelectedStockViewComponent);
            }
            double price;
            if (!double.TryParse(stockInfo["c"].ToString(), out price))
            {
                Console.WriteLine("Cannot convert current price to double");
                ViewBag.Errors = "Error while handling selected stocks request";
                return View(null as SelectedStockViewComponent);
            }

            SelectedStockViewModel viewModel = new SelectedStockViewModel()
            {
                Symbol = stockSymbol,
                ImageSrc = company["logo"].ToString(),
                Name = company["name"].ToString(),
                FinnhubIndustry = company["finnhubIndustry"].ToString(),
                Exchange = company["exchange"].ToString(),
                Price = price
            };
            return View(viewModel);
        }
    }
}
