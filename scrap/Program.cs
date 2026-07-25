using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using gemini.Services.MAD;
using gemini.Services.XOF;
using gemini.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using gemini.Repositories;
using gemini.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();

// builder.Services.AddScoped<MADParserService>();
// builder.Services.AddScoped<IScraperService, MADScraperService>();

builder.Services.AddScoped<IScraperService, XOFScraperService>();
builder.Services.AddScoped<XOFParserService>();


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
var scrapers = scope.ServiceProvider.GetServices<IScraperService>();

foreach (var scraper in scrapers)
{
    await scraper.ScrapeDateRange(new DateOnly(2020,1,1), DateOnly.FromDateTime(DateTime.Today));
}