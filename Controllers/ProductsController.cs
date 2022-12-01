using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DAL;
using Shop.Dapper.DAL.AppRepositories;
using Shop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers
{
    [Route("/api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private ProductsManager _productsManager;
        private DapperProductRepository _productsRepos;
        private ApplicationDbContext _ctx;

        public ProductsController(ApplicationDbContext ctx, ProductsManager productsManager, DapperProductRepository dapperProductRepository)
        {
            _productsManager = productsManager;
            _productsRepos = dapperProductRepository;
            _ctx = ctx;
        }

        [HttpGet("filtered-by-category/{categoryId}")]
        public IActionResult FilteredByCategory(int categoryId)
        {
            var filteredProducts = _productsRepos.GetProductsByCategoryId(categoryId);

            return Ok(filteredProducts);
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_productsRepos.GetProducts());

        [HttpGet("{productId}")]
        public IActionResult GetById(int productId) =>
            Ok(_productsRepos.GetProductById(productId));

        //[HttpGet("filtered-by-category/{categoryId}")]
        //public IActionResult FilteredByCategory(int categoryId)
        //{
        //    var filteredProducts = _ctx.Products.Include(p => p.Category)
        //                                .Include(p => p.Stocks)
        //                                .Where(x => x.CategoryId == categoryId).ToList();

        //    return Ok(filteredProducts);
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetAll() => Ok(await _productsManager.GetProductsWithStockAsync());

        //[HttpGet("{productId}")]
        //public async Task<IActionResult> GetById(int productId) =>
        //    Ok(await _ctx.Products
        //        .Include(x => x.Stocks)
        //        .Where(p => p.Id.Equals(productId))
        //        .FirstOrDefaultAsync());
    }
}
