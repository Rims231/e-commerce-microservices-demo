using eCommerce.Order.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Timeout;

namespace eCommerce.Order.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(
            this IServiceCollection services, IConfiguration config)
        {
            // Register HttpClient for Product API
            services.AddHttpClient<IOrderService, OrderService>(client =>
            {
                client.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Register Resilience Pipeline
            services.AddResiliencePipeline("my-pipeline", builder =>
            {
                builder
                    .AddRetry(new Polly.Retry.RetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        Delay = TimeSpan.FromSeconds(1),
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true
                    })
                    .AddTimeout(TimeSpan.FromSeconds(10));
            });

            return services;
        }
    }
}