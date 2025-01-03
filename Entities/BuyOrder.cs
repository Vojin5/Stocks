﻿using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class BuyOrder
    {
        [Key]
        public Guid BuyOrderID { get; set; }

        [Required]
        [StringLength(50)]
        public string? StockSymbol { get; set; }

        [Required]
        [StringLength(100)]
        public string? StockName { get; set; }

        public DateTime DateAndTimeOfOrder { get; set; }

        [Range(1,100000)]
        public int Quantity { get; set; }

        [Range(1,10000)]
        public double Price { get; set; }

        public override string ToString()
        {
            return $"Buy Order ID : {BuyOrderID}, Stock Symbol : {StockSymbol}, Stock Name : {StockName}, Date and time of Order : {DateAndTimeOfOrder}, Quantity : {Quantity}, Price : {Price}";
        }
    }
}
