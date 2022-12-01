using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DAL;
using Shop.Dapper.DAL.AppRepositories;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers
{
    [ApiController]
    //[Authorize(Policy = "mod")]
    [Route("api/adminOfStock")]
    public class StockAdminController : ControllerBase
    {
        private ApplicationDbContext _ctx;
        private DapperStocksRepository _dapperStocksRepository;
        private DapperProductRepository _dapperProductRepository;

        public StockAdminController(ApplicationDbContext ctx, DapperStocksRepository dapperStocksRepository,
            DapperProductRepository dapperProductRepository) 
        {
            _ctx = ctx;
            _dapperStocksRepository = dapperStocksRepository;
            _dapperProductRepository = dapperProductRepository;
        }

        [HttpGet("")]
        public IActionResult GetProductsWithStocksAsync()
        {
            var productsWithStocks = _dapperProductRepository.GerProductsWithStocks();
          
            //await _ctx.Products.Include(x => x.Stocks).ToArrayAsync();

            return Ok(productsWithStocks);
        }

        [HttpGet("{productId}")]
        public IActionResult GetStocksOfProduct(int productId)
        {
            var stocks = _dapperStocksRepository.GetStocksByProductId(productId);
            //var stock = (await _ctx
            //    .Products
            //    .Include(x => x.Stocks)
            //    .Where(p => p.ProductId == productId)
            //    .FirstOrDefaultAsync())
            //    .Stocks;

            //if(stock == null)
            //{
            //    return BadRequest($"Stock with id: {productId} not found");
            //}
            return Ok(new StockResult
            {
                ResultMessage = "Stock of product with id: " + productId,
                ResponseData = stocks,
            });
        }

        [HttpGet("{stockId}")]
        public IActionResult GetStockAsync(int stockId)
        {
            //var stockById = (await _ctx.Products
            //    .Include(x => x.Stocks)
            //    .Where(p => p.ProductId == productId) 
            //    .FirstOrDefaultAsync())
            //    .Stocks
            //    .Where(s => s.StockId == stockId)
            //    .FirstOrDefault();
            var stock = _dapperStocksRepository.GetStockById(stockId);

            return Ok(stock);
        }
            
        [HttpPost]
        public IActionResult CreateNewStock([FromBody] CreateNewStockModel requestStock )
        {
            if (requestStock == null)
                return BadRequest(new StockResult
                {
                    ResultMessage = "Create Stock Request Failed",
                    ResponseData = null
                });

            var newStock = new Stock
            {
                ProductId = requestStock.ProductId,
                Description = requestStock.StockDescription,
                Qty = requestStock.Qty
            };

            var stockId = _dapperStocksRepository.Create(ref newStock);

            return Ok(new StockResult
            {
                ResultMessage = "New Stock Created sucsesfully",
                ResponseData= newStock,
            });
        }

        [HttpDelete("{stockId}")]
        public async Task<IActionResult> RemoveStockAsync(int stockId)
        {
            var stock = await _ctx.Stocks.Where(s => s.StockId == stockId).FirstOrDefaultAsync();

            if(stock != null)
            {
                var result = _ctx.Stocks.Remove(stock);
                await _ctx.SaveChangesAsync();

                return Ok();
            }

            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStock(Stock editedStock)
        {
            var oldStock = await _ctx.Stocks.Where(x => x.StockId == editedStock.StockId).FirstOrDefaultAsync();

            oldStock.Description = editedStock.Description;
            oldStock.Qty = editedStock.Qty;

            await _ctx.SaveChangesAsync();

            return Ok();
        }

        public class CreateNewStockModel
        {
            public int ProductId { get; set; }
            public int Qty { get; set; }
            public string StockDescription { get; set; }
        }
        public class StockResult
        {
            public string ResultMessage { get; set; }
            public object ResponseData { get; set; }
        }

    }
}
