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
using Microsoft.Extensions.Logging;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false);

builder.Services.AddSerilog((services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration);
});

builder.Services.AddHttpClient();

builder.Services.AddScoped<IHttpClientProvider, HttpClientProvider>();
builder.Services.AddScoped<ISeleniumProvider, SeleniumProvider>();

builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();

/** 
* register MAD
*/
builder.Services.AddScoped<MADParserService>();
builder.Services.AddScoped<IParserService>(sp =>
    sp.GetRequiredService<MADParserService>());

builder.Services.AddScoped<MadProvider>();
builder.Services.AddScoped<ICurrencyProvider>(sp =>
    sp.GetRequiredService<MadProvider>());

builder.Services.AddScoped<IExchangeRateScraper>(sp =>
    new ExchangeRateScraper(
        sp.GetRequiredService<MadProvider>(),
        sp.GetRequiredService<ICurrencyRepository>(),
        sp.GetRequiredService<IExchangeRateRepository>(),
        sp.GetRequiredService<ILogger<ExchangeRateScraper>>()
    )
);

/** 
* register XOF
*/
builder.Services.AddScoped<XOFParserService>();
builder.Services.AddScoped<IParserService>(sp =>
    sp.GetRequiredService<XOFParserService>());

builder.Services.AddScoped<XofProvider>();
builder.Services.AddScoped<ICurrencyProvider>(sp =>
    sp.GetRequiredService<XofProvider>());

// builder.Services.AddScoped<IExchangeRateScraper>(sp =>
//     new ExchangeRateScraper(
//         sp.GetRequiredService<XofProvider>(),
//         sp.GetRequiredService<ICurrencyRepository>(),
//         sp.GetRequiredService<IExchangeRateRepository>()
//     )
// );

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

var scrapers = scope.ServiceProvider.GetServices<IExchangeRateScraper>();
foreach (var scraper in scrapers)
{
    // await scraper.ScrapeDateRange(new DateOnly(2020, 1, 1), DateOnly.FromDateTime(DateTime.Today), 2);
    await scraper.ScrapeDateRange(new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 5), 2);
    // await scraper.ScrapeLastDays(3);
}