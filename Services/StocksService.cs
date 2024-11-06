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

        public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            ModelValidator.Validate(request);
            BuyOrder order = request.ToBuyOrder();
            order.BuyOrderID = new Guid();
            BuyOrder responseCreate = await _stocksRepository.CreateBuyOrder(order);
            BuyOrderResponse response = responseCreate.ToBuyOrderResponse();
            return response;
        }

        public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? request)
        {
            if(request== null)
                throw new ArgumentNullException(nameof(request));

            ModelValidator.Validate(request);
            SellOrder order = request.ToSellOrder();
            order.SellOrderID = Guid.NewGuid();
            SellOrder responseCreate = await _stocksRepository.CreateSellOrder(order);
            SellOrderResponse response = responseCreate.ToSellOrderResponse();
            return response;
        }

        public async Task<List<BuyOrderResponse>> GetBuyOrders()
        {
            return (await _stocksRepository.GetBuyOrders()).Select(x => x.ToBuyOrderResponse()).ToList();
        }

        public async Task<List<SellOrderResponse>> GetSellOrders()
        {
            return (await _stocksRepository.GetSellOrders()).Select(x => x.ToSellOrderResponse()).ToList();
        }
    }
}
