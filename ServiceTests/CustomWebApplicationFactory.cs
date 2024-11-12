using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddUserSecrets<Program>();
            });
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<StocksDbContext>));
                if(descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddDbContext<StocksDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });
                var serviceProvider = services.BuildServiceProvider();
                using(var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetService<StocksDbContext>();
                    if(dbContext != null)
                        SeedDatabase(dbContext);
                }

                var basePath = Directory.GetCurrentDirectory();
                var solutionPath = Path.Combine(basePath, @"../../../../");
                var projectPath = Path.Combine(solutionPath, "./StocksApp");
                var wwwrootPath = Path.Combine(projectPath, "wwwroot");
                RotativaConfiguration.Setup(wwwrootPath, wkhtmltopdfRelativePath: "Rotativa");
            });
        }

        private void SeedDatabase(StocksDbContext context)
        {
            if (!context.BuyOrders.Any())
            {
                context.BuyOrders.Add(new BuyOrder()
                {
                    StockSymbol = "AAPL",
                    StockName = "Apple",
                    DateAndTimeOfOrder = DateTime.Now,
                    Quantity = 100,
                    Price = 100
                });
                context.BuyOrders.Add(new BuyOrder()
                {
                    StockSymbol = "MSFT",
                    StockName = "Microsoft",
                    DateAndTimeOfOrder = DateTime.Now,
                    Quantity = 100,
                    Price = 100
                });

                context.SellOrders.Add(new SellOrder()
                {
                    StockSymbol = "AAPL",
                    StockName = "Apple",
                    DateAndTimeOfOrder = DateTime.Now,
                    Quantity = 50,
                    Price = 100
                });
                context.SellOrders.Add(new SellOrder()
                {
                    StockSymbol = "MSFT",
                    StockName = "Microsoft",
                    DateAndTimeOfOrder = DateTime.Now,
                    Quantity = 50,
                    Price = 100
                });
                context.SaveChanges();
            }
        }
    }
}
