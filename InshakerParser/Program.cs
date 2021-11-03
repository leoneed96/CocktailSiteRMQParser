﻿using AngleSharp.Parser;
using Inshaker.Client;
using InshakerParser.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQTools;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace InshakerParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args);
            await host.RunConsoleAsync();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddLogging(x => x.AddConsole());

                    var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                    services.AddRabbitMQ(configuration);

                    services.Configure<MongoOptions>(configuration.GetSection(nameof(MongoOptions)));
                    services.AddHttpClient();

                    services.AddHostedService<CocktailListWorker>();
                    services.AddHostedService<DetailsWorker>();

                    services.AddSingleton<InshakerClient>();
                    services.AddSingleton<AngleSharpParser>();
                    services.AddSingleton<MongoConnection>();
                });
        }
    }
}
