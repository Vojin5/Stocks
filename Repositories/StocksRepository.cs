using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class StocksRepository : IStocksRepository
    {
        private readonly StocksDbContext _dbContext;
        public StocksRepository(StocksDbContext stocksDbContext)
        {
            _dbContext = stocksDbContext;
        }

        public async Task<BuyOrder?> CreateBuyOrder(BuyOrder buyOrder)
        {
            try
            {
                await _dbContext.BuyOrders.AddAsync(buyOrder);
                int changes = await _dbContext.SaveChangesAsync();
                if(changes > 0)
                {
                    return buyOrder;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<SellOrder?> CreateSellOrder(SellOrder sellOrder)
        {
            try
            {
                await _dbContext.SellOrders.AddAsync(sellOrder);
                int changes = await _dbContext.SaveChangesAsync();
                if(changes > 0)
                {
                    return sellOrder;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<List<BuyOrder>?> GetBuyOrders()
        {
            try
            {
                return await _dbContext.BuyOrders.ToListAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<List<SellOrder>?> GetSellOrders()
        {
            try
            {
                return await _dbContext.SellOrders.ToListAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
