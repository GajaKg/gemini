using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using gemini.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using gemini.Repositories;
using gemini.Services;
using gemini.Services.HtmlProviders;
using gemini.Services2;
using gemini.Services2.CurrencyProviders;
using gemini.Services2.HtmlProviders;
using gemini.Services.MAD;

var builder = Host.CreateApplicationBuilder(args);

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

// builder.Services.AddScoped<IExchangeRateScraper>(sp =>
//     new ExchangeRateScraper(
//         sp.GetRequiredService<MadProvider>(),
//         sp.GetRequiredService<ICurrencyRepository>(),
//         sp.GetRequiredService<IExchangeRateRepository>()
//     )
// );

/** 
* register XOF
*/
builder.Services.AddScoped<XOFParserService>();
builder.Services.AddScoped<IParserService>(sp =>
    sp.GetRequiredService<XOFParserService>());

builder.Services.AddScoped<XofProvider>();
builder.Services.AddScoped<ICurrencyProvider>(sp =>
    sp.GetRequiredService<XofProvider>());

builder.Services.AddScoped<IExchangeRateScraper>(sp =>
    new ExchangeRateScraper(
        sp.GetRequiredService<XofProvider>(),
        sp.GetRequiredService<ICurrencyRepository>(),
        sp.GetRequiredService<IExchangeRateRepository>()
    )
);


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

// var parser = scope.ServiceProvider.GetRequiredService<IParserService>();
var scrapers = scope.ServiceProvider.GetServices<IExchangeRateScraper>();

foreach (var scraper in scrapers)
{
    await scraper.ScrapeDateRange(new DateOnly(2020, 1, 1), DateOnly.FromDateTime(DateTime.Today), 2);
    // await scraper.ScrapeLastDays(3);
}