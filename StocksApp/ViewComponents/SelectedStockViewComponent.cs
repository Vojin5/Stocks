using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace StocksApp.ViewComponents
{
    public class SelectedStockViewComponent : ViewComponent
    {
        private readonly IFinnhubService _finnhubService;

        public SelectedStockViewComponent(IConfiguration configuration, IFinnhubService finnhubService)
        {
            _finnhubService = finnhubService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string stockSymbol)
        {
            Dictionary<string,object>? company = await _finnhubService.GetCompanyProfile(stockSymbol);
            Dictionary<string,object>? stockInfo = await _finnhubService.GetStockPriceQuote(stockSymbol);

            if (company == null || stockInfo == null)
                throw new Exception("Error while handling selected stock request");
            if (!company.ContainsKey("logo") || !company.ContainsKey("name") ||
                !company.ContainsKey("finnhubIndustry") || !company.ContainsKey("exchange"))
                throw new Exception("Error while getting company data");
            if (!stockInfo.ContainsKey("c"))
                throw new Exception("Error while getting current price of stock");
            double price;
            if (!double.TryParse(stockInfo["c"].ToString(), out price))
                throw new Exception("Cannot convert current price to double");

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
