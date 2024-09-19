using Entities.CustomValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class BuyOrderRequest
    {
        [Required]
        public string? StockSymbol { get; set; }


        [Required]
        public string? StockName { get; set; }


        [MinDate("2000-01-01")]
        public DateTime DateAndTimeOfOrder { get; set; }


        [Range(1,100000)]
        public int Quantity { get; set; }


        [Range(1,10000)]
        public double Price { get; set; }

        public BuyOrder ToBuyOrder()
        {
            return new BuyOrder()
            {
                StockSymbol = StockSymbol,
                StockName = StockName,
                DateAndTimeOfOrder = DateAndTimeOfOrder,
                Quantity = Quantity,
                Price = Price
            };
        }
    }
}
