using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class OrdersViewModel
    {
        public List<BuyOrderResponse> BuyOrders { get; set; } = new List<BuyOrderResponse>();
        public List<SellOrderResponse> SellOrders { get; set; } = new List<SellOrderResponse>();

        public override string ToString()
        {
            return $"{BuyOrders.ToString()}  {SellOrders.ToString()}";
        }
    }
}
