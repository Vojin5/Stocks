using Entities;
using Entities.DTO;
using Entities.HelperClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class StocksService : IStocksService
    {
        private readonly IStocksRepository _stocksRepository;
        private readonly ILogger<StocksService> _logger;

        public StocksService(IStocksRepository stocksRepository, ILogger<StocksService> logger)
        {
            _stocksRepository = stocksRepository;
            _logger = logger;

            _logger.LogInformation("Stock Service created");
        }

        public async Task<BuyOrderResponse?> CreateBuyOrder(BuyOrderRequest? request)
        {
            _logger.LogInformation("Stocks Service : CreateBuyOrder");
            
            if(request == null)
            {
                _logger.LogError("Stocks Service : CreateBuyOrder : Error : request is null");
                return null;
            }
            _logger.LogDebug($"Stocks Service : CreateBuyOrder request : {request.ToString()}");

            bool isValid = ModelValidator.IsValid(request);
            if(!isValid)
            {
                _logger.LogError("Stocks Service : CreateBuyOrder : Error : invalid request");
                return null;
            }

            BuyOrder order = request.ToBuyOrder();
            order.BuyOrderID = new Guid();
            BuyOrder? responseCreate = await _stocksRepository.CreateBuyOrder(order);
            if (responseCreate == null)
            {
                _logger.LogError("Stocks Service : CreateBuyOrder : Error : response is null");
                return null;
            }
            BuyOrderResponse response = responseCreate.ToBuyOrderResponse();

            _logger.LogInformation("Stocks Service : CreateBuyOrder : Success");
            _logger.LogDebug($"Stocks Service : CreateBuyOrder returns : {response.ToString()}");
            return response;
        }

        public async Task<SellOrderResponse?> CreateSellOrder(SellOrderRequest? request)
        {
            _logger.LogInformation("Stocks Service : CreateSellOrder");

            if (request == null)
            {
                _logger.LogError("Stocks Service : CreateSellOrder : Error : request is null");
                return null;
            }
            _logger.LogDebug($"Stocks Service : CreateSellOrder request : {request.ToString()}");

            bool isValid = ModelValidator.IsValid(request);
            if(!isValid)
            {
                _logger.LogError("Stocks Service : CreateSellOrder : Error : request is invalid");
                return null;
            }
            SellOrder order = request.ToSellOrder();
            order.SellOrderID = Guid.NewGuid();
            SellOrder? responseCreate = await _stocksRepository.CreateSellOrder(order);
            if(responseCreate == null)
            {
                _logger.LogError("Stocks Service : CreateSellOrder : Error : response is null");
                return null;
            }
            SellOrderResponse response = responseCreate.ToSellOrderResponse();

            _logger.LogInformation("Stocks Service : CreateSellOrder : Success");
            _logger.LogDebug($"Stocks Service : CreateSellOrder returns {response.ToString()}");
            return response;
        }

        public async Task<List<BuyOrderResponse>?> GetBuyOrders()
        {
            _logger.LogInformation("Stocks Service : GetBuyOrders");
            List<BuyOrder>? buyOrders = await _stocksRepository.GetBuyOrders();
            if(buyOrders == null)
            {
                _logger.LogError("Stocks Service : GetBuyOrders : Error : list of orders is null");
                return null;
            }

            _logger.LogInformation("Stocks Service : GetBuyOrders : Success");
            _logger.LogDebug($"Stocks Service : GetBuyOrders returns {buyOrders.ToString()}");
            return buyOrders.Select(x => x.ToBuyOrderResponse()).ToList();
        }

        public async Task<List<SellOrderResponse>?> GetSellOrders()
        {
            _logger.LogInformation("Stocks Service : GetSellOrders");
            List<SellOrder>? sellOrders = await _stocksRepository.GetSellOrders();
            if(sellOrders == null)
            {
                _logger.LogError("Stocks Service : GetSellOrders : Error : list of orders is null");
                return null;
            }
            _logger.LogInformation("Stocks Service : GetSellOrders : Success");
            _logger.LogDebug($"Stocks Service : GetSellOrders returns {sellOrders.ToString()}");
            return sellOrders.Select(x => x.ToSellOrderResponse()).ToList();
        }
    }
}
