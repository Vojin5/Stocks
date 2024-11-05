using Microsoft.Extensions.Configuration;
using RepositoryContracts;
using ServiceContracts;
using System.Text.Json;

namespace Services
{
    public class FinnhubService : IFinnhubService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IFinnhubRepository _finnhubRepository;
        
        public FinnhubService(HttpClient httpClient,IConfiguration configuration,IFinnhubRepository finnhubRepository)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _finnhubRepository = finnhubRepository;
        }
        public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol)
        {
            return await _finnhubRepository.GetCompanyProfile(stockSymbol);
        }

        public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
        {
            return await _finnhubRepository.GetStockPriceQuote(stockSymbol);
        }

        public async Task<List<Dictionary<string, string>>?> GetStocks()
        {
            return await _finnhubRepository.GetStocks();
        }

        public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
        {
            return await _finnhubRepository.SearchStocks(stockSymbolToSearch);
        }
    }
}
