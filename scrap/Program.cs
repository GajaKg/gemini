using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using gemini.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddScoped<IScraperService, MADScraperService>();
builder.Services.AddScoped<IParserService, ParserService>();

var host = builder.Build();

using var scope = host.Services.CreateScope();

var scraper = scope.ServiceProvider.GetRequiredService<IScraperService>();
var parser = scope.ServiceProvider.GetRequiredService<IParserService>();

await scraper.RunAsync();

Console.WriteLine("Hello, World!");
// Console.ReadKey();