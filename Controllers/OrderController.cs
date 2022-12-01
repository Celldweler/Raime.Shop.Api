using Microsoft.AspNetCore.Mvc;
using Shop.DAL;
using Shop.Domain.Dto.Order;
using Shop.Domain.Dto.Stock;
using Shop.Domain.Entities;
using Shop.Domain.Enums;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private StockManager _stockManager;
        private ApplicationDbContext _ctx;

        public OrderController(ApplicationDbContext ctx, StockManager stockManager)
        {
            _stockManager = stockManager;
            _ctx = ctx;
        }

        public class OrderVm
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CreateOrderDto order)
        {
            if(order.CustomerId != null)
            {
               var customerIsExist = _ctx.Customers.FirstOrDefault(x => x.Id.Equals(order.CustomerId));
                
               if(customerIsExist != null)
               {
                    foreach (var product in order.Products)
                    {
                        await _stockManager.RemoveQtyFromOneStockAsync(new RemoveStockDto
                        {
                            Qty = product.Qty,
                            StockId = _ctx.Stocks.FirstOrDefault(x =>
                                        x.ProductId == product.ProductId && x.Description == product.StockDescription).StockId,
                        });
                    }

                    await _ctx.Orders.AddAsync(new Order
                    {
                        CustomerId = order.CustomerId,
                        Created = DateTime.Now,
                        Status = OrderStatus.New,
                        IsCompleted = false,
                       
                        Products = order.Products.Select(y => new CartProduct
                        {
                            Qty = y.Qty,
                            StockDescription = y.StockDescription,
                            Amount = y.Price,
                            ProductId = y.ProductId,
                        }).ToList(),
                    });
                    await _ctx.SaveChangesAsync();
               }
               else
               {
                   // create new customer 
               }
            }

            return Ok();
        }
    }
}
