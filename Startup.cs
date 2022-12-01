using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raime.Shop.Api.Controllers.TestControllers;
using Raime.Shop.Api.EntitiesMapper;
using Raime.Shop.Api.Hubs;
using Raime.Shop.Api.IdentityServer4;
using Raime.Shop.Api.SeedData;
using Shop.DAL;
using Shop.Dapper.DAL.AppRepositories;
using Shop.Dapper.DAL.DbInteractionHelperServices;
using Shop.Dapper.DAL.Interfaces;
using System.Text.Json.Serialization;

namespace Raime.Shop.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;
        public Startup(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=ShopDb;Trusted_Connection=True";

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddRazorPages();
            services.AddCors(x => x.AddPolicy("All", p => p.AllowAnyHeader()
                                                           .AllowAnyMethod()
                                                           .SetIsOriginAllowed(_ => true)
                                                           .AllowAnyOrigin()));
            services.AddSingleton<ProductsStore>();
            services.AddTransient<ProductsManager>();
            services.AddTransient<StockManager>();
            
            // Dapper
            services.AddScoped<DapperProductRepository>();
            services.AddScoped<DapperStocksRepository>();
            services.AddScoped<DapperCategoriesRepository>();
            services.AddScoped<IEntityMapper, EntityMapper>();

            services.AddSingleton<TestUsersStore>();
            services.AddSingleton<TestCommentsStore>();

            services.AddTransient<MessageMapper>();

            //services.AddTransient<IStockManager, StockManager>();
            //services.AddSingleton<OrdersStore>();

            AddIdentity(services);
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("All");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }

        private void AddIdentity(IServiceCollection services)
        {
            var connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=ShopIdentityDB;Trusted_Connection=True";

            services.AddDbContext<AppIdentityDbContext>(config =>
            {
                config.UseSqlServer(connectionString);
                //config.UseInMemoryDatabase("DevIdentity");
            });


            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.LogoutPath = "/Account/Login";
                config.LogoutPath = "/api/auth/logout";
            });

            var identityServerBuilder = services.AddIdentityServer();

            identityServerBuilder.AddAspNetIdentity<IdentityUser>();

            if (env.IsDevelopment())
            {
                identityServerBuilder.AddInMemoryClients(IdentityServerConfiguration.Clients);
                identityServerBuilder.AddInMemoryIdentityResources(IdentityServerConfiguration.IdentityResources);
                identityServerBuilder.AddInMemoryApiScopes(IdentityServerConfiguration.ApiScopes);
                identityServerBuilder.AddDeveloperSigningCredential();
            }

            services.AddLocalApiAuthentication();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("mod", policy =>
                {
                    var is4Policy = options.GetPolicy(IdentityServerConstants.LocalApi.PolicyName);
                    policy.Combine(is4Policy);
                    policy.RequireClaim(RaimeShopConstants.Claims.Role, RaimeShopConstants.Roles.Mod);
                });
            });
        }
    }    
}
