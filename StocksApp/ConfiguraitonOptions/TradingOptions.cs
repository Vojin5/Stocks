namespace StocksApp.ConfiguraitonOptions
{
    public class TradingOptions
    {
        public int DefaultOrderQuantity { get; set; } 
        public string? DefaultStockSymbol { get; set; }
        public List<string> Top25PopularStocks { get; set; }
    }
}
