using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Dapper.DAL.AppRepositories;
using Shop.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shop.Dapper.DAL.DbInteractionHelperServices;

namespace Raime.Shop.Api.Controllers
{
    [Route("/api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private string _dir;
        private IWebHostEnvironment _env;
        private DapperProductRepository _productsRepos;

        public TestController(IWebHostEnvironment webHostEnvironment, DapperProductRepository productRepository)
        {
            _dir = Path.Combine(webHostEnvironment.ContentRootPath, @"wwwroot\images\product_images");
            _env = webHostEnvironment;
            _productsRepos = productRepository;
        }

        [HttpGet("orders")]
        public List<Order> Get()
        {
            return new DapperOrdersRepository()
                .GetOrders()
                .IncludeCustomer()
                .IncludeCartProducts();
        }

        [HttpGet("byCategoryId/{id}")]
        public List<Product> Get(int id)
        {
            return _productsRepos.GetProductsByCategoryId(id).ToList();
        }

        [HttpPost]
        public IActionResult Create([FromBody]Product product)
        {
            return Ok(_productsRepos.Insert(product));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _productsRepos.Delete(id);
            return Ok();
        }

        [HttpPut]
        public IActionResult Update([FromBody]Product product)
        {
            return Ok(_productsRepos.Update(product));
        }

        [HttpGet]
        public IActionResult Index() => BadRequest("test controller is work");

        //[HttpPost("")]
        //public IActionResult SingleFile(IFormFile file)
        //{
        //    using (var fileStream = new FileStream(Path.Combine(_dir, "file.png"),
        //        FileMode.Create, FileAccess.Write))
        //    {
        //        file.CopyTo(fileStream);
        //    }

        //    return Ok();
        //}

    }
}
