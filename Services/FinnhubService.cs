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
                return null;


            Dictionary<string,object>? result = await _finnhubRepository.GetCompanyProfile(stockSymbol);

            if (result.IsNullOrEmpty() || result == null)
                return null;
            if (result.ContainsKey("error"))
                return null;
            return result;
        }

        public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
        {
            string? apiKey = _configuration["FinnhubApiKey"];
            if (apiKey == null)
                throw new Exception("No Api Key provided, FinnhubApiKey secret");
            if (string.IsNullOrEmpty(stockSymbol))
                return null;

            Dictionary<string,object>? result =  await _finnhubRepository.GetStockPriceQuote(stockSymbol);

            if (result == null || result.IsNullOrEmpty())
                return null;
            if (result.ContainsKey("error"))
                return null;
            if (Convert.ToDouble(result["c"].ToString()) == 0)
                return null;

            return result;
        }

        public async Task<List<Dictionary<string, string>>?> GetStocks()
        {
            string? apiKey = _configuration["FinnhubApiKey"];
            if (apiKey == null)
                throw new Exception("No Api Key provided, FinnhubApiKey secret");

            List<Dictionary<string,string>>? result =  await _finnhubRepository.GetStocks();

            if (result == null || result.IsNullOrEmpty())
                return null;
            if (result.Count == 0)
                return null;
            if (result.Count == 1)
                return null;

            return result;
        }

        public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
        {
            string? apiKey = _configuration["FinnhubApiKey"];
            if (apiKey == null)
                throw new Exception("No Api Key provided, FinnhubApiKey secret");
            if (string.IsNullOrEmpty(stockSymbolToSearch))
                return null;

            Dictionary<string,object>? result =  await _finnhubRepository.SearchStocks(stockSymbolToSearch);

            if (result == null || result.IsNullOrEmpty())
                return null;
            if (result.Count == 0)
                return null;
            if (result.ContainsKey("error"))
                return null;

            return result;
        }
    }
}
