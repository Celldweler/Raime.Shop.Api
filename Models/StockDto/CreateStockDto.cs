using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Models.StockDto
{
    public class CreateStockDto
    {
        public string Description { get; set; }
        public int Count { get; set; }
    }
}
