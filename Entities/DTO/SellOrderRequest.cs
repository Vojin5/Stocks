using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Entities.CustomValidators;

namespace Entities.DTO
{
    public class SellOrderRequest
    {
        [Required]
        public string? StockSymbol { get; set; }


        [Required]
        public string? StockName { get; set; }


        [MinDate("2000-01-01")]
        public DateTime DateAndTimeOfOrder { get; set; }


        [Range(1, 100000)]
        public int Quantity { get; set; }


        [Range(1, 10000)]
        public double Price { get; set; }

        public SellOrder ToSellOrder()
        {
            return new SellOrder()
            {
                StockSymbol = StockSymbol,
                StockName = StockName,
                DateAndTimeOfOrder = DateAndTimeOfOrder,
                Price = Price,
                Quantity = Quantity
            };
        }

        public override string ToString()
        {
            return $"Stock symbol : {StockSymbol}, Stock name : {StockName}, Date and time of order : {DateAndTimeOfOrder}, Price : {Price}, Quantity : {Quantity}";
        }
    }
}
