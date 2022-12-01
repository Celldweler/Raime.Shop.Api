using System.Collections.Generic;

namespace Raime.Shop.Api.Models.ProductDto
{
    public class UpdateStockDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public ICollection<UpdateStockDto> Stocks { get; set; }
    }
}
