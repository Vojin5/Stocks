using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<StocksRepository> _logger;
        public StocksRepository(StocksDbContext stocksDbContext,ILogger<StocksRepository> logger)
        {
            _dbContext = stocksDbContext;
            _logger = logger;

            _logger.LogInformation("Stocks Repository created");
        }

        public async Task<BuyOrder?> CreateBuyOrder(BuyOrder buyOrder)
        {
            _logger.LogInformation("Stocks repository : CreateBuyOrder");
            _logger.LogDebug(buyOrder.ToString());
            try
            {
                await _dbContext.BuyOrders.AddAsync(buyOrder);
                int changes = await _dbContext.SaveChangesAsync();
                if(changes > 0)
                {
                    _logger.LogInformation($"Stocks Repository : CreateBuyOrder : Success");
                    _logger.LogDebug($"Stocks repository : CreateBuyOrder returns : {buyOrder.ToString()}");
                    return buyOrder;
                }
                else
                {
                    _logger.LogError($"Stocks Repository : CreateBuyOrder : error : No changes were made");
                    return null;
                }
            }
            catch(Exception e)
            {
                _logger.LogError($"Stocks repository : CreateBuyOrder : error : {e.Message}");
                return null;
            }
        }

        public async Task<SellOrder?> CreateSellOrder(SellOrder sellOrder)
        {
            _logger.LogInformation("Stocks repository : CreateSellOrder");
            _logger.LogDebug(sellOrder.ToString());
            try
            {
                await _dbContext.SellOrders.AddAsync(sellOrder);
                int changes = await _dbContext.SaveChangesAsync();
                if(changes > 0)
                {
                    _logger.LogInformation("Stocks repository : CreateSellOrder : Success");
                    _logger.LogDebug($"Stocks repository : CreateSellOrder returns {sellOrder.ToString()}");
                    return sellOrder;
                }
                else
                {
                    _logger.LogError("Stocks repository : CreateSellOrder : error : no changes were made");
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Stocks repository : CreateSellOrder : errror : {e.Message}");
                return null;
            }
        }

        public async Task<List<BuyOrder>?> GetBuyOrders()
        {
            _logger.LogInformation("Stocks repository : GetBuyOrders");
            try
            {
                List<BuyOrder> buyOrders = await _dbContext.BuyOrders.ToListAsync();
                _logger.LogDebug($"Stocks repository : GetBuyOrders returns {buyOrders}");
                return buyOrders;
            }
            catch(Exception e)
            {
                _logger.LogError($"Stocks repository : GetBuyOrders : error : {e.Message}");
                return null;
            }
        }

        public async Task<List<SellOrder>?> GetSellOrders()
        {
            _logger.LogInformation("Stocks repository : GetSellOrders");
            try
            {
                List<SellOrder> sellOrders = await _dbContext.SellOrders.ToListAsync();
                _logger.LogDebug($"Stocks repository : GetSellOrders returns {sellOrders}");
                return sellOrders;
            }
            catch(Exception e)
            {
                _logger.LogError($"Stocks repository : GetSellOrders : error : {e.Message}");
                return null;
            }
        }
    }
}
