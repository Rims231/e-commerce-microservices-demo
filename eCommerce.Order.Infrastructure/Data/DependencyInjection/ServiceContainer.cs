using eCommerce.Order.Application.Interfaces;
using eCommerce.Order.Infrastructure.Data;
using eCommerce.Order.Infrastructure.Repositories;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Order.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(
            this IServiceCollection services, IConfiguration config)
        {
            // Add SharedLibrary services (JWT + DbContext + Serilog + Middleware)
            SharedServiceContainer.AddSharedServices<OrderDbContext>(services, config,
                config["MySerilog:FileName"]!);

            // Register IOrder -> OrderRepository
            services.AddScoped<IOrder, OrderRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(
            this IApplicationBuilder app)
        {
            // Use SharedLibrary middleware (GlobalException + ListenToOnlyApiGateway)
            SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}