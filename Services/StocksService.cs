using Entities;
using Entities.DTO;
using Entities.HelperClasses;
using Microsoft.EntityFrameworkCore;
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
        private readonly StocksDbContext _db;

        public StocksService(StocksDbContext db)
        {
            _db = db;
        }

        public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            ModelValidator.Validate(request);
            BuyOrder order = request.ToBuyOrder();
            order.BuyOrderID = Guid.NewGuid();
            await _db.BuyOrders.AddAsync(order);
            await _db.SaveChangesAsync();
            BuyOrderResponse response = order.ToBuyOrderResponse();
            return response;
        }

        public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? request)
        {
            if(request== null)
                throw new ArgumentNullException(nameof(request));

            ModelValidator.Validate(request);
            SellOrder order = request.ToSellOrder();
            order.SellOrderID = Guid.NewGuid();
            await _db.SellOrders.AddAsync(order);
            await _db.SaveChangesAsync();
            SellOrderResponse response = order.ToSellOrderResponse();
            return response;
        }

        public async Task<List<BuyOrderResponse>> GetBuyOrders()
        {
            return await _db.BuyOrders.Select(x => x.ToBuyOrderResponse()).ToListAsync();
        }

        public async Task<List<SellOrderResponse>> GetSellOrders()
        {
            return await _db.SellOrders.Select(x => x.ToSellOrderResponse()).ToListAsync();
        }
    }
}
