using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(
            this IServiceCollection services, IConfiguration config)
        {
            // Add SharedLibrary services (JWT + DbContext + Serilog + Middleware)
            SharedServiceContainer.AddSharedServices<AuthenticationApiDbContext>(services, config,
                config["MySerilog:FileName"]!);

            // Register IUser -> UserRepository
            services.AddScoped<IUser, UserRepository>();

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