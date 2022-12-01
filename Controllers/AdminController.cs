using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raime.Shop.Api.Models.ProductDto;
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
    [Authorize(Policy = "mod")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private ApplicationDbContext _ctx;
        private DapperProductRepository _productsRepos;

        public AdminController(ApplicationDbContext ctx, DapperProductRepository dapperProductRepository)
        {
            _ctx = ctx;
            _productsRepos = dapperProductRepository;
        }

        [HttpGet]
        public  ActionResult<IEnumerable<Product>> Products() => _productsRepos.GetProducts().ToList();
        
        [HttpGet("{productId}")]
        public Product Product(int productId) => _productsRepos.GetProductById(productId);

        [HttpPost]
        public IActionResult CreateProductAsync([FromBody] CreateProductDto product)
        {
            var newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImagePath = product.Image,
                CategoryId = product.CategoryId,

                Stocks = product.Stocks.Select(s => new Stock
                {
                    Description = s.Description,
                    Qty = s.Count,
                }).ToList(),
            };

            var isSuccess = _productsRepos.Insert(newProduct);
            
            return Ok(isSuccess);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveProductAsync(int productId)
        {
            var productToRemoved = _ctx.Products.Where(p => p.ProductId == productId).FirstOrDefault();            
            var stocksOfProductToRemoved = _ctx.Stocks.Where(s => s.ProductId == productId).ToArray();

            if(stocksOfProductToRemoved != null)
            {
                _ctx.Stocks.RemoveRange(stocksOfProductToRemoved);
                _ctx.Products.Remove(productToRemoved);
            }
            else
            {
                _ctx.Products.Remove(productToRemoved);
            }
            await _ctx.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> EditProduct([FromBody] Product product)
        {
            var productToUpdate = _ctx.Products.Include(y => y.Stocks)
                .FirstOrDefault(x => x.ProductId == product.ProductId);

            if (product != null)
            {
                productToUpdate.Name = product.Name;
                productToUpdate.Description = product.Description;
                productToUpdate.Price = product.Price;
                //productToUpdate.Image = product.Image;
                productToUpdate.Stocks = new List<Stock>(product.Stocks);

                await _ctx.SaveChangesAsync();
            }
            else
                return BadRequest();

            return Ok();
        }
    }
}
