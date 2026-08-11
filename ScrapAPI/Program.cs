
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ScrapAPI.Data;
using ScrapAPI.Repositories;
using ScrapAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("ScraperDatabase"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorLocalhost",
        builder =>
        {
            builder.WithOrigins("http://localhost:5285", "https://localhost:5285")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // Optional, if using cookies/auth
        });
});

builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();


// Controllers
builder.Services.AddControllers();

// Api versioning
// builder.Services.AddApiVersioning(options =>
// {
//     options.DefaultApiVersion = new ApiVersion(1, 0);
//     options.AssumeDefaultVersionWhenUnspecified = true;
//     options.ReportApiVersions = true;
//     options.ApiVersionReader = new UrlSegmentApiVersionReader();
// });

// OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Middlewares
// builder.Services.AddTransient<GlobalExceptionMiddleware>();

// rate limiter
// The approach I just showed you has a problem - the rate limit policy is global and applies to all users. 
// All users share 10 request, if first user has 10 request others will be blocked. With rateLimitPartition every user has 10 request!
// Most of the time, you don't want to do this. Rate limiting should be granular and apply to individual users.
// Luckily, you can achieve this by creating a RateLimitPartition.
// The RateLimitPartition has two components:
// Partition key
// Rate limiter policy
// Rate limiting by IP address can be a good layer of security for unauthenticated users. 
// You don't know who is accessing your system and can't apply more granular rate limiting. 
// This can help protect your system from malicious users trying to perform a DDoS attack.
// builder.Services.AddRateLimiter(options =>
// {
//     options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

//     options.AddPolicy("fixed-by-ip", httpContext =>
//         RateLimitPartition.GetSlidingWindowLimiter(
//             partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
//             factory: _ => new SlidingWindowRateLimiterOptions
//             {
//                 SegmentsPerWindow = 6, // chunks window in 6 segments, performance optmization
//                 PermitLimit = 10, // The maximum number of requests allowed during one window.
//                 Window = TimeSpan.FromMinutes(1), // The length of the time window. | every 60 sec -> 10 request
//                 QueueLimit = 1, // if we have 15 req, first 10 will be proccessed, 2 will wait and rest will receive 429 response
//                 QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                 AutoReplenishment = true // after finish all 10 request automatically reset to 10 again, if false we need to reset manually which is uncommon for web api
//             }));

//     options.AddPolicy("fixed-by-user", httpContext =>
//         RateLimitPartition.GetSlidingWindowLimiter(
//             partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
//             factory: _ => new SlidingWindowRateLimiterOptions
//             {
//                 SegmentsPerWindow = 6,
//                 PermitLimit = 10,
//                 Window = TimeSpan.FromMinutes(1),
//                 QueueLimit = 1,
//                 QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                 AutoReplenishment = true
//             }));

// });

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Currency Scraper")
               .WithTheme(ScalarTheme.Moon)
               .WithDefaultHttpClient(
                   ScalarTarget.CSharp,
                   ScalarClient.HttpClient);
    });
}
// app.UseRateLimiter();

// app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowBlazorLocalhost");
app.MapControllers();

app.Run();