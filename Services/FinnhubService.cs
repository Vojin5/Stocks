using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<FinnhubService> _logger;
        
        public FinnhubService(IFinnhubRepository finnhubRepository,IConfiguration configuration,ILogger<FinnhubService> logger)
        {
            _finnhubRepository = finnhubRepository;
            _configuration = configuration;
            _logger = logger;

            _logger.LogInformation("Finnhub Service created");
        }
        public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol)
        {
            _logger.LogInformation($"Finnhub Service : GetCompanyProfile for {stockSymbol}");

            string? apiKey = _configuration["FinnhubApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogCritical("Finnhub Service : GetCompanyProfile : Critical : No Api Key provided in configuration");
                throw new Exception("No Api Key provided, FinnhubApiKey secret");
            }
            if (string.IsNullOrEmpty(stockSymbol))
            {
                _logger.LogError("Finnhub Service : GetCompanyProfile : Error : stock symbol is null or empty");
                return null;
            }


            Dictionary<string,object>? result = await _finnhubRepository.GetCompanyProfile(stockSymbol);

            if (result.IsNullOrEmpty() || result == null)
            {
                _logger.LogError("Finnhub Service : GetCompanyProfile : Error : result is null or empty");
                return null;
            }
            if (result.ContainsKey("error"))
            {
                _logger.LogError($"Finnhub Service : GetCompanyProfile : Error : error in result {result["error"].ToString()}");
                return null;
            }
            _logger.LogInformation("Finnhub Service : GetCompanyProfile : Success");
            _logger.LogDebug($"Finnhub Service : GetCompanyProfile returned {result.ToString()}");
            return result;
        }

        public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
        {
            _logger.LogInformation($"Finnhub Service : GetStockPriceQuote with {stockSymbol}");

            string? apiKey = _configuration["FinnhubApiKey"];
            if (apiKey == null)
            {
                _logger.LogCritical("Finnhub Service : Critical : No api key provided in configuration");
                throw new Exception("No Api Key provided, FinnhubApiKey secret");
            }
            if (string.IsNullOrEmpty(stockSymbol))
            {
                _logger.LogError("Finnhub Service : GetStockPriceQuote : Error : stock symbol is null or empty");
                return null;
            }

            Dictionary<string,object>? result =  await _finnhubRepository.GetStockPriceQuote(stockSymbol);

            if (result == null || result.IsNullOrEmpty())
            {
                _logger.LogError("Finnhub Service : GetStockPriceQuote : Error : result is null");
                return null;
            }
            if (result.ContainsKey("error"))
            {
                _logger.LogError($"Finnhub Service : GetStockPriceQuote : Error : result contains error : {result["error"].ToString()}");
                return null;
            }
            if (!result.ContainsKey("c") || Convert.ToDouble(result["c"].ToString()) == 0)
            {
                _logger.LogError("Finnhub Service : GetStocksPriceQuote : Error : result is 0");
                return null;
            }

            _logger.LogInformation("Finnhub Service : GetStocksPriceQuote : Success");
            _logger.LogDebug($"Finnhub Service : GetStocksPriceQuote returns {result.ToString()}");
            return result;
        }

        public async Task<List<Dictionary<string, string>>?> GetStocks()
        {
            _logger.LogInformation("Finnhub Service : GetStocks");

            string? apiKey = _configuration["FinnhubApiKey"];
            if (apiKey == null)
            {
                _logger.LogCritical("Finnhub Service : Critical : Api key is not provided in configuration");
                throw new Exception("No Api Key provided, FinnhubApiKey secret");
            }

            List<Dictionary<string,string>>? result =  await _finnhubRepository.GetStocks();

            if (result == null || result.IsNullOrEmpty())
            {
                _logger.LogError("Finnhub Service : GetStocks : Error : result is null or empty");
                return null;
            }
            if (result.Count == 0)
            {
                _logger.LogError("Finnhub Service : GetStocks : Error with result");
                return null;
            }
            if (result.Count == 1)
            {
                _logger.LogError("Finnhub Service : GetStocks : Error with result");
                return null;
            }

            _logger.LogInformation("Finnhub Service : GetStocks : Success");
            _logger.LogDebug($"Finnhub Service : GetStocks : returns {result.ToString()}");
            return result;
        }

        public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
        {
            _logger.LogInformation($"Finnhub Service : SearchStocks for {stockSymbolToSearch}");

            string? apiKey = _configuration["FinnhubApiKey"];
            if (apiKey == null)
            {
                _logger.LogCritical("Finnhub Service : Critical : No api key provided in configuration");
                throw new Exception("No Api Key provided, FinnhubApiKey secret");
            }
            if (string.IsNullOrEmpty(stockSymbolToSearch))
            {
                _logger.LogError("Finnhub Service : SearchStocks : Error : symbol is null or empty");
                return null;
            }

            Dictionary<string,object>? result =  await _finnhubRepository.SearchStocks(stockSymbolToSearch);

            if (result == null || result.IsNullOrEmpty())
            {
                _logger.LogError("Finnhub Service : SearchStocks : Error : result is null");
                return null;
            }
            if (result.Count == 0)
            {
                _logger.LogError("Finnhub Service : SearchStocks : Error : Error with result");
                return null;
            }
            if (result.ContainsKey("error"))
            {
                _logger.LogError("Finnhub Service : SearchStocks : Error : Error with result");
                return null;
            }

            _logger.LogInformation("Finnhub Service : SearchStocks : Success");
            _logger.LogDebug($"Finnhub Service : SearchStocks returns {result.ToString()}");
            return result;
        }
    }
}
