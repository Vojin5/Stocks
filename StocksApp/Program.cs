using ServiceContracts;
using Services;
using StocksApp.ConfiguraitonOptions;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IFinnhubService, FinnhubService>();
builder.Services.AddSingleton<IStocksService, StocksService>();

builder.Services.Configure<TradingOptions>
    (builder.Configuration.GetSection("TradingOptions"));

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();


app.Run();
