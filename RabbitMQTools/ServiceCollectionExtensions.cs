using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQTools.WorkingQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQTools
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<RabbitMQConnection>();
            services.AddSingleton<WorkingQueueWrapper>();
            services.Configure<RabbitMQOptions>(configuration.GetSection(nameof(RabbitMQOptions)));

            return services;
        }
    }
}
