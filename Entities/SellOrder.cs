using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SellOrder
    {
        [Key]
        public Guid SellOrderID { get; set; }

        [Required]
        [StringLength(50)]
        public string? StockSymbol { get; set; }

        [Required]
        [StringLength(100)]
        public string? StockName { get; set; }

        public DateTime DateAndTimeOfOrder { get; set; }

        [Range(1,100000)]
        public int Quantity { get; set; }

        [Range(1, 10000)]
        public double Price { get; set; }

        public override string ToString()
        {
            return $"Buy Order ID : {SellOrderID}, Stock Symbol : {StockSymbol}, Stock Name : {StockName}, Date and time of Order : {DateAndTimeOfOrder}, Quantity : {Quantity}, Price : {Price}";
        }
    }
}
