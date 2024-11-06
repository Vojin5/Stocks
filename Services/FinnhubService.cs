using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryContracts;
using ServiceContracts;
using System.Text.Json;

namespace Services
{
    public class FinnhubService : IFinnhubService
    {        
        private readonly IFinnhubRepository _finnhubRepository;
        private readonly IConfiguration _configuration;
        
        public FinnhubService(IFinnhubRepository finnhubRepository,IConfiguration configuration)
        {
            _finnhubRepository = finnhubRepository;
            _configuration = configuration;
        }
        public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol)
        {
            string? apiKey = _configuration["FinnhubApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("No Api Key provided, FinnhubApiKey secret");
            if (string.IsNullOrEmpty(stockSymbol))
                throw new ArgumentException("No symbol provided");


            Dictionary<string,object>? result = await _finnhubRepository.GetCompanyProfile(stockSymbol);

            if (result.IsNullOrEmpty() || result == null)
                throw new InvalidOperationException("Empty response from finnhub");
            if (result.ContainsKey("error"))
                throw new InvalidOperationException(result["error"].ToString());
            return result;
        }

        public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
        {
            string? apiKey = _configuration["FinnhubApiKey"];
            if (apiKey == null)
                throw new Exception("No Api Key provided, FinnhubApiKey secret");
            if (string.IsNullOrEmpty(stockSymbol))
                throw new ArgumentException("No symbol provided");

            Dictionary<string,object>? result =  await _finnhubRepository.GetStockPriceQuote(stockSymbol);

            if (result == null || result.IsNullOrEmpty())
                throw new InvalidOperationException("No response from Finnhub");
            if (Convert.ToDouble(result["c"].ToString()) == 0)
                throw new InvalidOperationException("Invalid input");
            if (result.ContainsKey("error"))
                throw new InvalidOperationException(result["error"].ToString());

            return result;
        }

        public async Task<List<Dictionary<string, string>>?> GetStocks()
        {
            string? apiKey = _configuration["FinnhubApiKey"];

            List<Dictionary<string,string>>? result =  await _finnhubRepository.GetStocks();

            if (result == null || result.IsNullOrEmpty())
                throw new InvalidOperationException("No response from Finnhub");
            if (result.Count == 0)
                throw new InvalidOperationException("Invalid input for symbol or api key");
            if (result.Count == 1)
                throw new InvalidOperationException("Provide api key in configuration");

            return result;
        }

        public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
        {
            string? apiKey = _configuration["FinnhubApiKey"];
            if (apiKey == null)
                throw new Exception("No Api Key provided, FinnhubApiKey secret");
            if (string.IsNullOrEmpty(stockSymbolToSearch))
                throw new ArgumentException("No symbol provided");

            Dictionary<string,object>? result =  await _finnhubRepository.SearchStocks(stockSymbolToSearch);

            if (result == null || result.IsNullOrEmpty())
                throw new InvalidOperationException("No response from Finnhub");
            if (result.Count == 0)
                throw new InvalidOperationException("Invalid input for symbol or api key");
            if (result.ContainsKey("error"))
                throw new InvalidOperationException(result["error"].ToString());

            return result;
        }
    }
}
