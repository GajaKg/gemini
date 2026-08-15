using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ScrappyCoco;
using ScrappyCoco.Services.Currencies;
using ScrappyCoco.Services.ExchangeRates;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5022")
});


builder.Services.AddTransient<IExchangeRateService, ExchangeRateService>();
builder.Services.AddTransient<ICurrenciesService, CurrenciesService>();

await builder.Build().RunAsync();
