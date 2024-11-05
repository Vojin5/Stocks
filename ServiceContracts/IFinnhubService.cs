namespace ServiceContracts
{
    public interface IFinnhubService
    {
        /// <summary>
        /// Gets the information of company based on stock symbol provided
        /// </summary>
        /// <param name="stockSymbol">Stock symbol for informations</param>
        /// <returns>Dictionary of found company profile</returns>
        Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol);

        /// <summary>
        /// Gets the price information of a specific stock
        /// </summary>
        /// <param name="stockSymbol">stock symbol for search</param>
        /// <returns>Dictionary of price information of provided stock</returns>
        Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol);

        /// <summary>
        /// Gets all the available stocks 
        /// </summary>
        /// <returns>List of dictionaries that are available on finnhub</returns>
        Task<List<Dictionary<string, string>>?> GetStocks();

        /// <summary>
        /// Returns the results of a search based on the provided stock symbol
        /// </summary>
        /// <param name="stockSymbolToSearch"></param>
        /// <returns></returns>
        Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch);

    }
}
