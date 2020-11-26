using Crpg.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Crpg.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services,
            IConfiguration configuration, IHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddDbContext<CrpgDbContext>(options => options.UseInMemoryDatabase("crpg"));
            }
            else
            {
                var connectionString = configuration.GetConnectionString("Crpg");
                services.AddDbContext<CrpgDbContext>(options =>
                    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
            }

            services.AddScoped<ICrpgDbContext>(provider => provider.GetRequiredService<CrpgDbContext>());

            return services;
        }
    }
}
