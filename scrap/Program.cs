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
using gemini.Services.Email;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false);

builder.Services.AddSerilog((services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration);
});

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IHttpClientProvider, HttpClientProvider>();
builder.Services.AddSingleton<ISeleniumProvider, SeleniumProvider>();

builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();

/** 
* register MAD
*/
builder.Services.AddScoped<IMadParserService, MADParserService>();
builder.Services.AddScoped<IMadCurrencyProvider, MadProvider>();
builder.Services.AddScoped<ICurrencyProvider>(sp =>
    sp.GetRequiredService<MadProvider>());

builder.Services.AddScoped<IExchangeRateScraper>(sp =>
    new ExchangeRateScraper(
        sp.GetRequiredService<IMadCurrencyProvider>(),
        sp.GetRequiredService<ICurrencyRepository>(),
        sp.GetRequiredService<IExchangeRateRepository>(),
        sp.GetRequiredService<ILogger<ExchangeRateScraper>>()
    )
);

/** 
* register XOF
*/
builder.Services.AddScoped<IXofParserService, XOFParserService>();
builder.Services.AddScoped<IXofCurrencyProvider, XofProvider>();
builder.Services.AddScoped<ICurrencyProvider>(sp =>
    sp.GetRequiredService<XofProvider>());

builder.Services.AddScoped<IExchangeRateScraper>(sp =>
    new ExchangeRateScraper(
        sp.GetRequiredService<IXofCurrencyProvider>(),
        sp.GetRequiredService<ICurrencyRepository>(),
        sp.GetRequiredService<IExchangeRateRepository>(),
        sp.GetRequiredService<ILogger<ExchangeRateScraper>>()
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

var scrapers = scope.ServiceProvider.GetServices<IExchangeRateScraper>();
foreach (var scraper in scrapers)
{
    // await scraper.ScrapeDateRange(new DateOnly(2020, 1, 1), DateOnly.FromDateTime(DateTime.Today), 2);
    // await scraper.ScrapeDateRange(new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 5));
    await scraper.ScrapeLastDays(10);
}
