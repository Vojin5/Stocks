using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RepositoryContracts;
using System.Numerics;
using System.Text.Json;

namespace Repositories
{
    public class FinnhubRepository : IFinnhubRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FinnhubRepository> _logger;

        public FinnhubRepository(HttpClient httpClient, IConfiguration configuration, ILogger<FinnhubRepository> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _logger.LogInformation("Finnhub Repository created");
        }
        public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol)
        {
            _logger.LogInformation($"Finnhub Repository : GetCompanyProfile for {stockSymbol}");

            try
            {
                string? apiKey = _configuration["FinnhubApiKey"];

                HttpRequestMessage requestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(
                        $"https://finnhub.io/api/v1/stock/profile2?symbol={stockSymbol}" +
                        $"&token={apiKey}")
                };

                HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);

                Stream stream = await responseMessage.Content.ReadAsStreamAsync();
                StreamReader reader = new StreamReader(stream);
                string response = await reader.ReadToEndAsync();

                Dictionary<string, object>? result =
                    JsonSerializer.Deserialize<Dictionary<string, object>>(response);

                _logger.LogInformation("Finnhub Repository : GetCompanyProfile Successful");
                return result;
            }catch(Exception e)
            {
                _logger.LogError($"Finnhub Repository : GetCompanyProfile error : {e.Message}");
                return null;
            }
        }

        public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
        {
            _logger.LogInformation($"Finnhub repository : GetStockPriceQuote for {stockSymbol}");

            try
            {
                string? apiKey = _configuration["FinnhubApiKey"];

                HttpRequestMessage requestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(
                        $"https://finnhub.io/api/v1/quote?symbol={stockSymbol}&token={apiKey}")
                };

                HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);


                Stream stream = await responseMessage.Content.ReadAsStreamAsync();
                StreamReader reader = new StreamReader(stream);
                string response = await reader.ReadToEndAsync();

                Dictionary<string, object>? result =
                    JsonSerializer.Deserialize<Dictionary<string, object>>(response);
                _logger.LogInformation("Finnhub repository : GetStockPriceQuote : Success");
                return result;
            }
            catch(Exception e)
            {
                _logger.LogError($"Finnhub repository : GetStockPriceQuote : error :{e.Message}");
                return null;
            }
        }

        public async Task<List<Dictionary<string, string>>?> GetStocks()
        {
            _logger.LogInformation("Finnhub repository : GetStocks");
            try
            {
                string? apiKey = _configuration["FinnhubApiKey"];

                HttpRequestMessage requestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(
                        $"https://finnhub.io/api/v1/stock/symbol?exchange=US&token={apiKey}")
                };

                HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);


                Stream stream = await responseMessage.Content.ReadAsStreamAsync();
                StreamReader reader = new StreamReader(stream);
                string response = await reader.ReadToEndAsync();

                List<Dictionary<string, string>>? result =
                    JsonSerializer.Deserialize<List<Dictionary<string, string>>>(response);

                _logger.LogInformation("Finnhub repository : GetStocks : Success");
                return result;
            }
            catch(Exception e)
            {
                _logger.LogError($"Finnhub repository : GetStocks : error : {e.Message}");
                return null;
            }

        }

        public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
        {
            _logger.LogInformation($"Finnhub repository : SearchStocks for {stockSymbolToSearch}");
            try
            {
                string? apiKey = _configuration["FinnhubApiKey"];

                HttpRequestMessage requestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(
                        $"https://finnhub.io/api/v1/search?q={stockSymbolToSearch}&token={apiKey}")
                };

                HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);


                Stream stream = await responseMessage.Content.ReadAsStreamAsync();
                StreamReader reader = new StreamReader(stream);
                string response = await reader.ReadToEndAsync();

                Dictionary<string, object>? result =
                    JsonSerializer.Deserialize<Dictionary<string, object>>(response);

                _logger.LogInformation("Finnhub repository : Search Stocks : Success");
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Finnhub repository : Search Stock : error : {e.Message}");
                return null;
            }
        }
    }
}
