using API.Models;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Producer;

namespace API
{

    /// <summary>
    /// A static class to perform the dependemcy injection.
    /// </summary>
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            var config = new ConfigurationBuilder().AddUserSecrets("8f282f66-069f-4706-940f-723c2210f397").Build();

            services.AddAutoMapper(typeof(MappingProfiles));

            services.AddDbContext<TestdbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("TestdbContext"));
            });

            services.AddScoped<IDatabaseServices, DatabaseServices>();

            services.AddSingleton<IProducerService, ProducerService>();

            //Injecting API Services
            services.AddScoped<IAPIServices, APIServices>();
            return services;
        }
    }
}
