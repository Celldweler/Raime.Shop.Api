using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shop.DAL;
using Shop.Domain.Entities;
using Shop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Raime.Shop.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var identityContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var appDbCtx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                //CustomerName = c.Users.FirstOrDefault(u => u.UserId != UserId && c.Id == u.ChatId).Customer.ToCustomerFullName(),

                //var user =  userManager.FindByIdAsync("test-user-73").GetAwaiter().GetResult();
                //userManager.AddClaimAsync(user, new Claim("phone_number", "+38 097 271 91 89")).GetAwaiter().GetResult();
                //var user = new IdentityUser("testUser")
                //{
                //    Id = "test-user-73",
                //    Email = "test@fmail.com",

                //};
                //userManager.CreateAsync(user, "password").GetAwaiter().GetResult();
                //userManager.AddClaimsAsync(user, new List<Claim>
                //{
                //    new Claim("address", "Pishonivs'ka St, 27, Odesa, Odes'ka oblast, 65000"),
                //    new Claim("phone", "+38 097 271 91 89")
                //})
                //.GetAwaiter().GetResult(); 

                if (!identityContext.Users.Any())
                {
                    var fakeUsersCounter = 3;

                    var fakeUsers = Enumerable.Range(0, fakeUsersCounter)
                        .Select(i => new IdentityUser($"fake{i}") { Id = $"fake_{i}_id", Email=$"fake{i}@test.com"})
                        .ToList();
                    foreach (var fakeuser in fakeUsers)
                    {
                        userManager.CreateAsync(fakeuser, "password").GetAwaiter().GetResult();
                    }

                    //var user = new IdentityUser("test");
                    //userManager.CreateAsync(user, "password").GetAwaiter().GetResult();

                    //var mod = new IdentityUser("mod");
                    //userManager.CreateAsync(mod, "admin").GetAwaiter().GetResult();
                    //userManager.AddClaimAsync(mod, new Claim(RaimeShopConstants.Claims.Role, RaimeShopConstants.Roles.Mod));
                }

                var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var pictures=new[]
                {
                    "Img__GLOCKS__JEANS.png",
                    "PIC_PREDATOR_VEST.png",
                    "PIC_BELTS_DISTRESSED_JEANS_DARK.png",
                    //"666-TEE WH.jfif",
                    //"D.O.C.-FULL ZIP PUFFER JKT BL.jpeg",
                    //"W.O.F.-FULL ZIP HOODIE BL.jpeg",
                    //"XCROSS-T WASHED HOODIE DARK GR.jfif",
                };

                //ctx.Database.EnsureDeletedAsync().GetAwaiter().GetResult();
                var categories = new List<Category>
                {
                    new Category { Name = "PANTS",  },
                    new Category { Name = "JACKETS",  },
                    new Category { Name = "TEES",},
                    new Category { Name = "HOODIES" },
                };
                if (!ctx.Categories.Any())
                {
                    ctx.Categories.AddRangeAsync(categories).GetAwaiter().GetResult();
                    ctx.SaveChangesAsync().GetAwaiter().GetResult();
                }
                if (!ctx.Products.Any() && !ctx.Customers.Any() && !ctx.Orders.Any())
                {
                    var products = new List<Product>
                        {
                            new Product
                            {
                                Name = "GLOCKS A312 JEANS D GR",
                                Description="Rap fit. Water repellent fabric. Glock suede patches + embroidered logo and text. Removable lil bag. Genuine leather branded embossing. YKK zippers. 82% cotton, 18% polyester.",
                                Price = 105,
                                ImagePath = pictures[0],
                                CategoryId = categories[0].CategoryId,
                                Stocks = new List<Stock>
                                {
                                    new Stock { Qty=100, Description = "XS", },
                                    new Stock { Qty=100, Description = "S",  },
                                    new Stock { Qty=100, Description = "M",  },
                                    new Stock { Qty=100, Description = "L",  },
                                    new Stock { Qty=100, Description = "XL", },
                                    new Stock { Qty=100, Description = "XXL",},
                                }
                            },
                            new Product
                            {
                                Name="PREDATOR VEST BL",
                                Description="Screen printed patch + embroidered logo. 3D glock. YKK zipper. 96% cotton, 4% polyester.",
                                Price=100,
                                ImagePath = pictures[1],
                                CategoryId = categories[1].CategoryId,
                                Stocks = new List<Stock>
                                {
                                    new Stock { Qty = 100, Description = "XS", },
                                    new Stock { Qty = 100, Description = "S",  },
                                    new Stock { Qty = 100, Description = "M",  },
                                    new Stock { Qty = 100, Description = "L",  },
                                    new Stock { Qty = 100, Description = "XL", },
                                    new Stock { Qty = 100, Description = "XXL",},
                                }
                            },
                            new Product
                            {
                                Name="BELTS DISTRESSED JEANS DARK GR",
                                Description="Rap Pants",
                                Price=98,
                                ImagePath = pictures[2],
                                CategoryId = categories[0].CategoryId,

                                //Category = categories[0],
                                Stocks = new List<Stock>
                                {
                                    new Stock { Qty = 100, Description = "XS", },
                                    new Stock { Qty = 100, Description = "S",  },
                                    new Stock { Qty = 100, Description = "M",   },
                                    new Stock { Qty = 100, Description = "L",  },
                                    new Stock { Qty = 100, Description = "XL", },
                                    new Stock { Qty = 100, Description = "XXL",},
                                }
                            }
                        };

                    ctx.Products.AddRangeAsync(products).GetAwaiter().GetResult();

                    ctx.SaveChangesAsync().GetAwaiter().GetResult();

                        var customer = new Customer
                        {
                            Id = "fake_1_id",
                            Name = "Dima",
                            Surname = "Ignatov",
                            Email = "domest@email.com",
                            PhoneNumber = "+380 98-22-11-431",
                            Adress = "Kanatna St, 98, Odesa, Odes'ka oblast, 65000",
                            City = "Odessa",
                            Country = "Ukraine",
                            PostCode = "65000",
                        };
                        var customer2 = new Customer
                        {
                            Id = "fake_0_id",
                            Name = "Artem",
                            Surname = "Naumov",
                            Email = "test@email.com",
                            PhoneNumber = "+380 97-27-19-189",

                            Adress = "Pishonivs'ka St, 27, Odesa, Odes'ka oblast, 65000",
                            City = "Odessa",
                            Country = "Ukraine",
                            PostCode = "65000",
                        };
                        var customer3 = new Customer
                        {
                            Id = "fake_2_id",
                            Name = "Akim",
                            Surname = "Semka",
                            Email = "prettyaki73@femail.com",
                            PhoneNumber = "+380 95-23-21-132",

                            Adress = "Panteleimonivs'ka St, 112, Odesa, Odes'ka oblast, 65000",
                            City = "Odessa",
                            Country = "Ukraine",
                            PostCode = "65000",
                        };
                    ctx.Customers.AddRangeAsync(new[] { customer, customer2, customer3, }).GetAwaiter().GetResult();
                    ctx.SaveChangesAsync().GetAwaiter().GetResult();

                    var orders = new List<Order>
                        {
                            new Order
                            {
                                CustomerId = customer2.Id, //"fake_0_id",

                                Created = DateTime.Now,
                                Status = OrderStatus.New,
                                IsCompleted = false,
                                Products = new List<CartProduct>
                                {
                                    new CartProduct{ProductId=products[0].ProductId,Amount=105,Qty=3,StockDescription="XL"},
                                    new CartProduct{ProductId = products[1].ProductId,Amount = 100,StockDescription="M",Qty = 4},
                                    new CartProduct{ProductId = products[2].ProductId,Amount = 98,StockDescription="S",Qty = 1},
                                }
                            },
                            new Order
                            {
                                CustomerId =  customer.Id, //"fake_1_id",
                                Created = DateTime.Now,
                                Status = OrderStatus.New,
                                IsCompleted = false,
                                Products = new List<CartProduct>{
                                    new CartProduct{ProductId = products[1].ProductId,Amount=100,StockDescription="XS",Qty=1},
                                }
                            },
                            new Order
                            {
                                CustomerId = customer.Id,   //"fake_1_id",
                                Created = DateTime.Now,
                                Status = OrderStatus.New,
                                IsCompleted = false,

                                Products = new List<CartProduct> {
                                    new CartProduct{ProductId=products[0].ProductId,StockDescription="M",Qty=3,Amount=105},
                                    new CartProduct{ProductId=products[1].ProductId,StockDescription="XL",Qty=1,Amount=100}
                                }
                            },
                            new Order
                            {
                                CustomerId = customer3.Id,   //"fake_2_id",
                                Created = DateTime.Now,
                                Status = OrderStatus.New,
                                IsCompleted = false,

                                Products = new List<CartProduct> {
                                    new CartProduct{ProductId= products[0].ProductId,StockDescription="M",Qty=3,Amount=105},
                                    new CartProduct{ProductId = products[2].ProductId, StockDescription = "XL", Qty = 1, Amount = 100}
                                }
                            },
                        };
                    ctx.AddRangeAsync(orders).GetAwaiter().GetResult();
                    ctx.SaveChangesAsync().GetAwaiter().GetResult();
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
