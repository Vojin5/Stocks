using Microsoft.Extensions.Configuration;
using ServiceContracts;
using System.Text.Json;

namespace Services
{
    public class FinnhubService : IFinnhubService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        
        public FinnhubService(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol)
        {
            string? apiKey = _configuration["FinnhubApiKey"];
            if (apiKey == null)
                throw new Exception("No Api Key provided, FinnhubApiKey secret");

            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(
                    $"https://finnhub.io/api/v1/stock/profile2?symbol={stockSymbol}" +
                    $"&token={apiKey}")
            };

            HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);

            if (!responseMessage.IsSuccessStatusCode)
                throw new Exception(responseMessage.StatusCode.ToString());

            Stream stream = await responseMessage.Content.ReadAsStreamAsync();
            StreamReader reader = new StreamReader(stream);
            string response = await reader.ReadToEndAsync();

            Dictionary<string, object>? result =
                JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            if (result == null)
                throw new InvalidOperationException("No response from Finnhub");
            if(result.Count == 0)
                throw new InvalidOperationException("Invalid input for symbol or api key");
            if (result.ContainsKey("error"))
                throw new InvalidOperationException(result["error"].ToString());

            return result;
        }

        public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
        {
            string? apiKey = _configuration["FinnhubApiKey"];
            if (apiKey == null)
                throw new Exception("No Api Key provided, FinnhubApiKey secret");

            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(
                    $"https://finnhub.io/api/v1/quote?symbol={stockSymbol}&token={apiKey}")
            };

            HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);

            if (!responseMessage.IsSuccessStatusCode)
                throw new Exception(responseMessage.StatusCode.ToString());

            Stream stream = await responseMessage.Content.ReadAsStreamAsync();
            StreamReader reader = new StreamReader(stream);
            string response = await reader.ReadToEndAsync();

            Dictionary<string,object>? result = 
                JsonSerializer.Deserialize<Dictionary<string,object>>(response);
            if (result == null)
                throw new InvalidOperationException("No response from Finnhub");
            if (Convert.ToDouble(result["c"].ToString()) == 0)
                throw new InvalidOperationException("Invalid input");
            if (result.Count == 0)
                throw new InvalidOperationException("Invalid input for symbol or api key");
            if (result.ContainsKey("error"))
                throw new InvalidOperationException(result["error"].ToString());

            return result;
        }
    }
}
