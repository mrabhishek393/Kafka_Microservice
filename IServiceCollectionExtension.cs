using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Producer;

namespace ConsumerApp
{

    /// <summary>
    /// A static class to perform the dependemcy injection.
    /// </summary>
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            var config = new ConfigurationBuilder().AddUserSecrets("8f282f66-069f-4706-940f-723c2210f397").Build();

            services.AddDbContext<TestdbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("minimaldbContext"));
            });

            services.AddScoped<IDatabaseServices, DatabaseServices>();

            services.AddSingleton<IProducerService, ProducerService>();

            //injecting consumer services
            services.AddSingleton<IConsumerServices, ConsumerServices>();

            //injecting consumer background services
            services.AddHostedService<ConsumerBackgroundService>();
            return services;
        }
    }
}
