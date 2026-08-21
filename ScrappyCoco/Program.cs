using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ScrappyCoco;
using ScrappyCoco.Services.Currencies;
using ScrappyCoco.Services.ExchangeRates;
using MudBlazor.Services;
using ScrapAPI.Infrastructure.RetryPolicies;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();

// builder.Services.AddScoped(sp => new HttpClient
// {
//     BaseAddress = new Uri("http://localhost:5022")
// });

builder.Services
    .AddHttpClient<IExchangeRateService, ExchangeRateService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5022");
    })
    .AddPolicyHandler((sp, _) => RetryPolicies.GetRetryPolicy());

builder.Services
    .AddHttpClient<ICurrenciesService, CurrenciesService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5022");
    })
    .AddPolicyHandler((sp, _) =>   RetryPolicies.GetRetryPolicy());


await builder.Build().RunAsync();
