using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DAL;
using Shop.Domain.Dto.CartProduct;
using Shop.Domain.Dto.Customer;
using Shop.Domain.Dto.Order;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Raime.Shop.Api.Controllers
{
    [Route("api/user-profile")]
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName)]
    public class UserAccountController : ApiController
    {
        private ApplicationDbContext _ctx;
        private UserManager<IdentityUser> _userManager;
       
        public UserAccountController(UserManager<IdentityUser> userManager, ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        [HttpGet("")]
        public IActionResult GetMe()
        {
            var userId = UserId;
            var customerInfo = _ctx.Customers
                                .Include(x => x.Orders)
                                .ThenInclude(y => y.Products)
                                .ThenInclude(p => p.Product)
                                .FirstOrDefault(x => x.Id.Equals(userId));
           
            if (customerInfo == null)
                return BadRequest();

            CustomerDto customerDto = new CustomerDto
            {
                Id = userId,
                Name = customerInfo.Name,
                Surname = customerInfo.Surname,
                Email = customerInfo.Email,
                PhoneNumber = customerInfo.PhoneNumber,

                Adress = customerInfo.Adress,
                City = customerInfo.City,
                Country = customerInfo.Country,
                PostCode = customerInfo.PostCode,

                Orders = customerInfo.Orders.Select(x => new OrderDTO
                {
                    Id = x.Id,
                    Created = x.Created,
                    Status = x.Status,
                    IsCompleted = x.IsCompleted,

                    Products = x.Products.Select(y => new CartProductDto
                    {
                        ProductTitle = y.Product.Name,
                        Price = y.Amount,
                        StockDescription = y.StockDescription,
                        Image = y.Product.ImagePath,
                        Qty = y.Qty,
                    }).ToList()

                }).ToList()
            };

            return Ok(customerDto);
        }


        [HttpPut]
        public IActionResult EditUserAccountInfo()
        {
            throw new NotImplementedException();
        }

        public class UpdateUserInfo
        {
        }
    }
}
