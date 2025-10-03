using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using Tannit.Application.Accounts;
using Tannit.Application.Payments;
using Tannit.Application.Payments.DTOs;
using Tannit.Infrastructure.Payments;
using Tannit.Infrastructure.Persistence;
using Tannit.Infrastructure.Repositories;

namespace Tannit.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //DB
            services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            // TODO: ajouter PropertiesRepository, InvestmentsRepository, etc.

            // config stripe
            // Stripe settings (binding appsettings.json → StripeSettings class)
            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
            // Initialise Stripe API key (global pour Stripe SDK)
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            // register implementation
            services.AddScoped<IPaymentGateway, StripePaymentGateway>();

            return services;
        }
    }
}
