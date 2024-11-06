using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class SelectedStockViewModel
    {
        public string? Symbol {  get; set; }
        public string? ImageSrc { get; set; }
        public string? Name { get; set; }
        public string? FinnhubIndustry { get; set; }
        public string? Exchange {  get; set; }
        public double? Price { get; set; }
    }
}
