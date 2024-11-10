using Entities;
using Entities.DTO;
using Entities.HelperClasses;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class StocksService : IStocksService
    {
        private readonly IStocksRepository _stocksRepository;

        public StocksService(IStocksRepository stocksRepository)
        {
            _stocksRepository = stocksRepository;
        }

        public async Task<BuyOrderResponse?> CreateBuyOrder(BuyOrderRequest? request)
        {
            if(request == null)
                return null;

            bool isValid = ModelValidator.IsValid(request);
            if(!isValid)
                return null;

            BuyOrder order = request.ToBuyOrder();
            order.BuyOrderID = new Guid();
            BuyOrder? responseCreate = await _stocksRepository.CreateBuyOrder(order);
            if (responseCreate == null)
                return null;
            BuyOrderResponse response = responseCreate.ToBuyOrderResponse();
            return response;
        }

        public async Task<SellOrderResponse?> CreateSellOrder(SellOrderRequest? request)
        {
            if (request == null)
                return null;

            bool isValid = ModelValidator.IsValid(request);
            if(!isValid)
                return null;
            SellOrder order = request.ToSellOrder();
            order.SellOrderID = Guid.NewGuid();
            SellOrder? responseCreate = await _stocksRepository.CreateSellOrder(order);
            if(responseCreate == null)
                return null;
            SellOrderResponse response = responseCreate.ToSellOrderResponse();
            return response;
        }

        public async Task<List<BuyOrderResponse>?> GetBuyOrders()
        {
            List<BuyOrder>? buyOrders = await _stocksRepository.GetBuyOrders();
            if(buyOrders == null)
                return null;
            return buyOrders.Select(x => x.ToBuyOrderResponse()).ToList();
        }

        public async Task<List<SellOrderResponse>?> GetSellOrders()
        {
            List<SellOrder>? sellOrders = await _stocksRepository.GetSellOrders();
            if(sellOrders == null)
                return null;
            return sellOrders.Select(x => x.ToSellOrderResponse()).ToList();
        }
    }
}
