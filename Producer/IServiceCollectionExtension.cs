using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    /// <summary>
    /// A static class to perform the dependemcy injection.
    /// </summary>
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddProducerConnector(this IServiceCollection services)
        {
            // Adding user secrets to the app
            var config = new ConfigurationBuilder().AddUserSecrets("8f282f66-069f-4706-940f-723c2210f397").Build();

            //Injecting Producer Service
            services.AddSingleton<IProducerService, ProducerService>();

            return services;
        }
    }
}
