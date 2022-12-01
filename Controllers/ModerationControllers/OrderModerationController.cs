using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raime.Shop.Api.Services;
using Shop.DAL;
using Shop.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers.ModerationControllers
{
    [Route("api/orders-moderation")]
    [ApiController]
    public class OrderModerationController : ControllerBase
    {
        private ApplicationDbContext _ctx;
        public OrderModerationController(ApplicationDbContext ctx) => _ctx = ctx;


        [HttpGet]
        public IActionResult GettAll()
        {
            var customers = _ctx.Customers.Include(x => x.Orders).ToList();
            var orders = _ctx.Orders.Include(x => x.Products).ToArray();
            var moderationOrdersVm = new List<ModerationOrderVm>();

            moderationOrdersVm = orders.Select(x => new ModerationOrderVm
            {
                Id = x.Id,
                Created = x.Created.ToDateCreatedViewModel(),
                Status = x.Status,
                IsCompleted = x.IsCompleted,
                CustomerId = x.CustomerId,

                CustomerInfo = new CustomerVm
                {
                    Id = customers.First(c => c.Id == x.CustomerId).Id,
                    Name = customers.First(c => c.Id == x.CustomerId).Name,
                    Surname = customers.First(c => c.Id == x.CustomerId).Surname,
                    SecondName = customers.First(c => c.Id == x.CustomerId).SecondName,
                    Email = customers.First(c => c.Id == x.CustomerId).Email,
                    PhoneNumber = customers.First(c => c.Id == x.CustomerId).PhoneNumber,

                    Adress = customers.First(c => c.Id == x.CustomerId).Adress,
                    City = customers.First(c => c.Id == x.CustomerId).City,
                    Country = customers.First(c => c.Id == x.CustomerId).Country,
                    PostCode = customers.First(c => c.Id == x.CustomerId).PostCode,
                },
                Products = orders
                    .Where(order => order.CustomerId == x.CustomerId)
                    .First().Products
                    .Select(vm => new CartProductVm
                    {
                        Id = vm.Id,
                        Qty = vm.Qty,
                        StockDescription = vm.StockDescription,
                        Amount = vm.Amount,
                        ProductId = vm.ProductId,
                        Product = _ctx.Products
                            .Where(p => p.ProductId.Equals(vm.ProductId))
                            .Select(pVm => new ProductVm
                            {
                                Name = pVm.Name,
                                Description = pVm.Description,
                                Price = pVm.Price,
                                Image = pVm.ImagePath,
                            }).First(),
                        OrderId = vm.OrderId,
                    }).ToList(),
            }).ToList();

            return Ok(moderationOrdersVm);
        }
        public class CustomerVm
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string SecondName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }

            public string Adress { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string PostCode { get; set; }
        }
        public class ModerationOrderVm
        {
            public int Id { get; set; }
            public string Created { get; set; }
            public OrderStatus Status { get; set; }
            public bool IsCompleted { get; set; }
            public string CustomerId { get; set; }
            public CustomerVm CustomerInfo { get; set; }

            public IEnumerable<CartProductVm> Products { get; set; } = new List<CartProductVm>();
        }
        public class CartProductVm
        {
            public int Id { get; set; }

            public int Qty { get; set; }
            public string StockDescription { get; set; }
            public decimal Amount { get; set; } 
            public int ProductId { get; set; }
            public ProductVm Product { get; set; }

            public int OrderId { get; set; }
        }
        public class ProductVm
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Description { get; set; }
            public string Image { get; set; }

            public string CategoryName { get; set; }
        }
        
        //public static class OrderStatus
        //{
        //    public const string Pending = "Pending";
        //    public const string AwaitingPayment = "AwaitingPayment";
        //    public const string AwaitingFulFillment = "AwaitingFulFillment";
        //    public const string AwaitingShipment = "AwaitingShipment";
        //    public const string Shipped = "Shipped";
        //    public const string Delivered = "Delivered";
        //    public const string AwaitingPickup = "AwaitingPickup";
        //    public const string Completed = "Completed";

        //    //Pending, //Новый заказ
        //    //AwaitingPayment,       // ожыдание оплаты
        //    //AwaitingFulFillment, // ожыдание выполнения комплектуется
        //    //AwaitingShipment, // комплектуется перевозчиком ожыдается к отправке
        //    //Shipped,            // отправлен следует к получателю
        //    //Delivered, // Доставлен
        //    //AwaitingPickup, // прибыл к получателю в ожыдании получателя на почте
        //    //Completed,      // заказ выполнен товар получен получателем
        //}

    }
}
