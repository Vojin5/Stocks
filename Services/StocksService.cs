using Entities;
using Entities.DTO;
using Entities.HelperClasses;
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
        private readonly List<BuyOrder> _buyOrderList;
        private readonly List<SellOrder> _sellOrderList;

        public StocksService()
        {
            _buyOrderList = new List<BuyOrder>();
            _sellOrderList = new List<SellOrder>();
        }

        public Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            ModelValidator.Validate(request);
            BuyOrder order = request.ToBuyOrder();
            order.BuyOrderID = Guid.NewGuid();
            _buyOrderList.Add(order);
            BuyOrderResponse response = order.ToBuyOrderResponse();
            return Task.FromResult(response);
        }

        public Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? request)
        {
            if(request== null)
                throw new ArgumentNullException(nameof(request));

            ModelValidator.Validate(request);
            SellOrder order = request.ToSellOrder();
            order.SellOrderID = Guid.NewGuid();
            _sellOrderList.Add(order);
            SellOrderResponse response = order.ToSellOrderResponse();
            return Task.FromResult(response);
        }

        public Task<List<BuyOrderResponse>> GetBuyOrders()
        {

            return Task.FromResult(_buyOrderList.Select(x => x.ToBuyOrderResponse()).ToList());
        }

        public Task<List<SellOrderResponse>> GetSellOrders()
        {
            return Task.FromResult(_sellOrderList.Select(x => x.ToSellOrderResponse()).ToList());
        }
    }
}
