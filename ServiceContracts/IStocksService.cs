using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IStocksService
    {
        /// <summary>
        /// Inserts a new buy order into the database table called 'BuyOrders'.
        /// </summary>
        /// <param name="request">BuyOrderRequest object</param>
        /// <returns>BuyOrderResponse object</returns>
        Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? request);
        /// <summary>
        ///  Inserts a new sell order into the database table called 'SellOrders'.
        /// </summary>
        /// <param name="request">SellOrderRequest object</param>
        /// <returns>SellOrderResponse object</returns>
        Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? request);
        /// <summary>
        /// Returns the existing list of buy orders retrieved from database table called 'BuyOrders'.
        /// </summary>
        /// <returns>List of BuyOrderResponse objects</returns>
        Task<List<BuyOrderResponse>> GetBuyOrders();
        /// <summary>
        /// Returns the existing list of sell orders retrieved from database table called 'SellOrders'.
        /// </summary>
        /// <returns>List of SellOrderResponse objects</returns>
        Task<List<SellOrderResponse>> GetSellOrders();
    }
}
