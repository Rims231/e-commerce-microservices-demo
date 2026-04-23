using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(
            this IServiceCollection services,
            IConfiguration config,
            string fileName) where TContext : DbContext
        {
            // Add JWT Authentication
            services.AddJWTAuthenticationScheme(config);

            // Add Generic Database Context
            services.AddDbContext<TContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("eCommerceConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));

            // Configure Serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(
                    path: $"{fileName}-.text",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            // Use global exception middleware
            app.UseMiddleware<GlobalException>();

            // Block all requests that are not from the API Gateway
            app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;
        }
    }
}