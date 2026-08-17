using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using gemini.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using gemini.Repositories;
using gemini.Services;
using gemini.Services.HtmlProviders;
using gemini.Services.CurrencyProviders;
using gemini.Services.CurrencyParser;
using Serilog;
using gemini.Services.Email;
using gemini.Application;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false);

builder.Services.AddSerilog((services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration);
});

builder.Services.AddHttpClient();

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddSingleton<IHttpClientProvider, HttpClientProvider>();
builder.Services.AddSingleton<ISeleniumProvider, SeleniumProvider>();

builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();

builder.Services.AddScoped<ScrapingRunner>();

/** 
* register MAD
*/
builder.Services.AddScoped<IMadParserService, MADParserService>();
builder.Services.AddScoped<IMadCurrencyProvider, MadProvider>();
builder.Services.AddScoped<IExchangeRateScraper, ExchangeRateScraper<IMadCurrencyProvider>>();

/** 
* register XOF
*/
builder.Services.AddScoped<IXofParserService, XOFParserService>();
builder.Services.AddScoped<IXofCurrencyProvider, XofProvider>();
builder.Services.AddScoped<IExchangeRateScraper, ExchangeRateScraper<IXofCurrencyProvider>>();


// Database
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("ScraperDatabase"));
});


var host = builder.Build();

using var scope = host.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
await db.Database.MigrateAsync();



// running app
var runner = scope.ServiceProvider.GetRequiredService<ScrapingRunner>();
// await runner.RunScrapeLastDaysAsync();
await runner.RunScrapeDateRangeAsync();