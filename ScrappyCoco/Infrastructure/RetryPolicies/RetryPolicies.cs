using Polly;
using Polly.Extensions.Http;

namespace ScrappyCoco.Infrastructure.RetryPolicies;

public class RetryPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(3));
    }
}