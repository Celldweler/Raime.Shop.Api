using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DAL;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers
{
    [Route("customer-wish-list")]
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName)]
    public class CustomerWishsListController : ApiController
    {
        private ApplicationDbContext _ctx;

        public CustomerWishsListController(ApplicationDbContext ctx) => _ctx = ctx;

        [HttpGet]
        public IActionResult GetWishList()
        {
            var customerWishList = _ctx.CustomerWishList
                .Include(p => p.Product)
                .Where(x => x.CustomerId.Equals(UserId))
                .Select(y => new WishItemResponse
                {
                    Id = y.Id,
                    ProductId = y.ProductId,
                    ProductName = y.Product.Name,
                    ProductPrice = y.Product.Price.ToString(),
                    Image = y.Product.ImagePath
                }).ToList();

            return Ok(customerWishList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWishItemAsync([FromBody] WishItemRequest wishItemRequest )
        {
            if (wishItemRequest == null) return BadRequest();

            var checkExistProduct = _ctx.CustomerWishList.FirstOrDefault(x => x.ProductId == wishItemRequest.ProductId);
            var product = _ctx.Products.FirstOrDefault(x => x.ProductId == wishItemRequest.ProductId);

            if (checkExistProduct != null)
                return BadRequest("Продукт с таким ID уже добавлен в ваш список желаний!");

            var customer = await _ctx.Customers.FirstOrDefaultAsync(x => x.Id.Equals(UserId));
           
            if (customer == null) return BadRequest();
            
            var wishItemNew = new WishItem
            {
                ProductId = wishItemRequest.ProductId,
                CustomerId = UserId,
            };

            customer.WishList.Add(wishItemNew);
            await _ctx.SaveChangesAsync();

            return Ok(new WishItemResponse
            {
                Id= wishItemNew.Id,
                ProductId = product.ProductId,
                ProductName= product.Name,
                ProductPrice= product.Price.ToString(),
                Image= product.ImagePath
            });
        }
    }

    public class WishItemResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductPrice { get; set; }
        public string Image { get; set; }
    }
    public class WishItemRequest
    {
        public int ProductId { get; set; }
    }
}
