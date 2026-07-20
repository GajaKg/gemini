using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using gemini.Services;
using scrap.Services.XOF;
using gemini.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();

// builder.Services.AddScoped<IScraperService, MADScraperService>();
builder.Services.AddScoped<IScraperService, XOFScraperService>();

// Database
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("ScraperDatabase"));
});


builder.Services.AddScoped<IParserService, XOFParserService>();

var host = builder.Build();

using var scope = host.Services.CreateScope();


var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

await db.Database.MigrateAsync();

// var parser = scope.ServiceProvider.GetRequiredService<IParserService>();
var scrapers = scope.ServiceProvider.GetServices<IScraperService>();

// foreach (var scraper in scrapers)
// {
//     await scraper.RunAsync();
// }
Console.WriteLine("Hello, World!");
// Console.ReadKey();