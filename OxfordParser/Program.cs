using OxfordParser.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using OxfordParser.Services;
using AngleSharp.Parser;
using Oxford.Client;

namespace OxfordParser
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

                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();

                    services.AddOptions();
                    services.Configure<FileStorageOptions>(configuration.GetSection(nameof(FileStorageOptions)));

                    var loggingConfig = new LoggerConfiguration()
                        .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                        .WriteTo.File("log.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error);

                    services.AddLogging(x => x.AddSerilog(loggingConfig.CreateLogger() , dispose: true));
                    services.AddDbContextFactory<WordsDbContext>(x => x.UseNpgsql(configuration.GetConnectionString(nameof(WordsDbContext))));
                    services.AddHttpClient();

                    services.AddSingleton<FileStorageService>();
                    services.AddSingleton<WordListPageParser>();
                    services.AddSingleton<WordDetailsParser>();
                    services.AddSingleton<OxfordClient>();

                    services.AddHostedService<WordListWorker>();
                });
        }
    }
}
