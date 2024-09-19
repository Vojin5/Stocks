using Entities.CustomValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class SellOrderResponse
    {
        [Required]
        public Guid SellOrderID { get; set; }


        [Required]
        public string? StockSymbol { get; set; }


        [Required]
        public string? StockName { get; set; }


        [MinDate("2000-01-01")]
        public DateTime DateAndTimeOfOrder { get; set; }


        [Range(1, double.MaxValue, ErrorMessage = "Quantity value invalid")]
        public int Quantity { get; set; }


        [Range(1, double.MaxValue, ErrorMessage = "Price value invalid")]
        public double Price { get; set; }


        [Range(1, double.MaxValue, ErrorMessage = "Trade Amount value invalid")]
        public double TradeAmount { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var compareObject = (SellOrderResponse)obj;
            return SellOrderID == compareObject.SellOrderID &&
                StockSymbol == compareObject.StockSymbol &&
                StockName == compareObject.StockName &&
                DateAndTimeOfOrder == compareObject.DateAndTimeOfOrder &&
                Quantity == compareObject.Quantity &&
                Price == compareObject.Price &&
                TradeAmount == compareObject.TradeAmount;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class SellOrderExtension
    {
        public static SellOrderResponse ToSellOrderResponse(this SellOrder sellOrder)
        {
            return new SellOrderResponse()
            {
                SellOrderID = sellOrder.SellOrderID,
                StockSymbol = sellOrder.StockSymbol,
                StockName = sellOrder.StockName,
                DateAndTimeOfOrder = sellOrder.DateAndTimeOfOrder,
                Quantity = sellOrder.Quantity,
                Price = sellOrder.Price,
                TradeAmount = sellOrder.Price * sellOrder.Quantity
            };
        }
    }
}
