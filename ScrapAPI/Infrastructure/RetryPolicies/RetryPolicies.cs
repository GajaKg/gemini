using Polly;
using Polly.Extensions.Http;

namespace ScrapAPI.Infrastructure.RetryPolicies;

public class RetryPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
    {
        return HttpPolicyExtensions
       .HandleTransientHttpError()
       .WaitAndRetryAsync(
           3,
           retryAttempt => TimeSpan.FromSeconds(1),
           (outcome, delay, retryAttempt, context) =>
           {
               logger.LogWarning(
                   "Retry {RetryAttempt}. Waiting {Delay} seconds. Status: {StatusCode}",
                   retryAttempt,
                   delay.TotalSeconds,
                   outcome.Result?.StatusCode
               );
           });
    }
}