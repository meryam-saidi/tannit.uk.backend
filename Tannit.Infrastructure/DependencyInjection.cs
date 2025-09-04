using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tannit.Application.Accounts;
using Tannit.Infrastructure.Persistence;
using Tannit.Infrastructure.Repositories;

namespace Tannit.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            // TODO: ajouter PropertiesRepository, InvestmentsRepository, etc.
            return services;
        }
    }
}
