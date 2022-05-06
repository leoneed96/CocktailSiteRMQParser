using OxfordParser.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
                    services.AddLogging(x => x.AddConsole());

                    services.AddDbContextFactory<WordsDbContext>(x => x.UseNpgsql(configuration.GetConnectionString(nameof(WordsDbContext))));
                });
        }
    }
}
