namespace RepositoryContracts
{
    public interface IFinnhubRepository
    {
        /// <summary>
        /// Gets the dictionary with company information of specified stockSymbol
        /// </summary>
        /// <param name="stockSymbol">symbol used to search company</param>
        /// <returns>Dictionary with company information</returns>
        Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol); 

        /// <summary>
        /// Gets the information of the specified stockSymbol 
        /// </summary>
        /// <param name="stockSymbol">stock symbol for search</param>
        /// <returns>the dictionary with stock data</returns>
        Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol);

        /// <summary>
        /// Lists the supported stocks
        /// </summary>
        /// <returns>List of dictionaries with available stocks</returns>
        Task<List<Dictionary<string, string>>?> GetStocks();

        /// <summary>
        /// Performs a search that returns the list of stocks based on query
        /// </summary>
        /// <param name="stockSymbolToSearch">search field</param>
        /// <returns>List of searched stocks based on input</returns>
        Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch);
    }
}
