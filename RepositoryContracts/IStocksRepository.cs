using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    public interface IStocksRepository
    {
        /// <summary>
        /// Inserts a new buy order into the database table BuyOrders
        /// </summary>
        /// <param name="buyOrder">New Buy order</param>
        /// <returns>The same buyOrder object</returns>
        Task<BuyOrder> CreateBuyOrder(BuyOrder buyOrder);

        /// <summary>
        /// Inserts a new sell order into the database table SellOrders
        /// </summary>
        /// <param name="sellOrder">New Sell order</param>
        /// <returns>The same sellOrder object</returns>
        Task<SellOrder> CreateSellOrder(SellOrder sellOrder);

        /// <summary>
        /// Returns all BuyOrders from database
        /// </summary>
        /// <returns>List of buy order objects</returns>
        Task<List<BuyOrder>> GetBuyOrders();

        /// <summary>
        /// Returns all SellOrders from database
        /// </summary>
        /// <returns>List of sell order objects</returns>
        Task<List<SellOrder>> GetSellOrders();
    }
}
