using IronProgrammerBotWebApplication.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace IronProgrammerBotWebApplication.Services
{
    public class ConfigureWebhook(
        ILogger<ConfigureWebhook> logger,
        IServiceProvider serviceProvider,
        IOptions<BotConfiguration> botOptions) : IHostedService
    {
        private readonly ILogger<ConfigureWebhook> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly BotConfiguration _botConfig = botOptions.Value;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var url = $"{_botConfig.HostAddress}{_botConfig.Route}";
            _logger.LogInformation("Url: {WebhookAddress}", url);
            await botClient.SetWebhookAsync(
                url: url,
                cancellationToken: cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            _logger.LogInformation("Удаление webhook");
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}
