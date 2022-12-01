using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Models.OrderDto
{
    public class OrderChangeStatusDto
    {
        public int OrderId { get; set; }
        public int Status { get; set; }
    }
}
