using Entities.CustomValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class BuyOrderResponse
    {
        [Required]
        public Guid BuyOrderID { get; set; }


        [Required]
        public string? StockSymbol { get; set; }


        [Required]
        public string? StockName { get; set; }


        [MinDate("2000-01-01")]
        public DateTime DateAndTimeOfOrder { get; set; }


        [Range(1,int.MaxValue,ErrorMessage = "Quantity value invalid")]
        public int Quantity { get; set; }


        [Range(1,double.MaxValue,ErrorMessage = "Price value invalid")]
        public double Price { get; set; }


        [Range(1, double.MaxValue, ErrorMessage = "Trade Amount value invalid")]
        public double TradeAmount { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj == null)
                return false;
            if(obj.GetType() != this.GetType())
            {
                return false;
            }

            var compareObject = (BuyOrderResponse)obj;
            return BuyOrderID == compareObject.BuyOrderID &&
                StockSymbol == compareObject.StockSymbol &&
                StockName == compareObject.StockName &&
                DateAndTimeOfOrder == compareObject.DateAndTimeOfOrder &&
                Quantity == compareObject.Quantity &&
                Price == compareObject.Price &&
                TradeAmount == compareObject.TradeAmount;
        }

        public override string ToString()
        {
            return $"Buy Order ID : {BuyOrderID}, Stock symbol : {StockSymbol}, Stock Name : {StockName}, Date and time of order : {DateAndTimeOfOrder}, Quantity : {Quantity}, Price : {Price}, Trade amount : {TradeAmount}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class BuyOrderExtension
    {
        public static BuyOrderResponse ToBuyOrderResponse(this BuyOrder buyOrder)
        {
            return new BuyOrderResponse()
            {
                BuyOrderID = buyOrder.BuyOrderID,
                StockName = buyOrder.StockName,
                StockSymbol = buyOrder.StockSymbol,
                Price = buyOrder.Price,
                Quantity = buyOrder.Quantity,
                DateAndTimeOfOrder = buyOrder.DateAndTimeOfOrder,
                TradeAmount = buyOrder.Quantity * buyOrder.Price
            };
        }
    }
}
