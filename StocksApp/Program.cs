using ServiceContracts;
using Services;
using StocksApp.ConfiguraitonOptions;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IFinnhubService, FinnhubService>();
builder.Services.AddScoped<IStocksService, StocksService>();
builder.Services.AddScoped<IFinnhubRepository, FinnhubRepository>();
builder.Services.AddScoped<IStocksRepository, StocksRepository>();
builder.Services.AddMemoryCache();

//http logging
builder.Services.AddHttpLogging(options => { });

//-trading options
builder.Services.Configure<TradingOptions>
    (builder.Configuration.GetSection("TradingOptions"));
//-db context
builder.Services.AddDbContext<StocksDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});

//serilog setup
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration logger) =>
{
    logger.ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
});

var app = builder.Build();

app.UseHttpLogging();

if(!builder.Environment.IsEnvironment("Test"))
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();


app.Run();

public partial class Program { }