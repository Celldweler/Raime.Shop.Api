namespace Raime.Shop.Api.Models.StockDto
{
    public class UpdateStockDto
    {
        public int ProductId { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }
}
