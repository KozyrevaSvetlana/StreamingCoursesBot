using IRON_Infrastructure_Common;
using IRON_PROGRAMMER_BOT_ConsoleApp.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IRON_PROGRAMMER_BOT_ConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
            {
                var botConfigurationSection = context.Configuration.GetSection(BotConfiguration.SectionName);
                var token = botConfigurationSection.GetSection("BotToken")!.Value;
                var firebaseConfigurationSection = context.Configuration.GetSection(FirebaseConfiguration.SectionName);
                var connection = context.Configuration.GetSection(ConnectionStringsConfiguration.SectionName);
                var baseDatabase = connection.GetSection("dbConnection")!.Value;
                ContainerConfigurator.Configure(services, token!, firebaseConfigurationSection, baseDatabase!);
                services.AddHostedService<LongPoolingConfigurator>();

            }).Build();
            await ContainerConfigurator.Initialize(host.Services);

            await host.RunAsync();
        }
    }
}
