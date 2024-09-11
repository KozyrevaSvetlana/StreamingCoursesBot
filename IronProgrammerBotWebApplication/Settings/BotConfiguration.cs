namespace IronProgrammerBotWebApplication.Settings
{
    public class BotConfiguration
    {
        public static readonly string Configuration = "BotConfiguration";
        public const string SectionName = "BotConfiguration";
        public string BotToken { get; init; } = default!;
        public string HostAddress { get; init; } = default!;
        public string Route { get; init; } = default!;
        public string SecretToken { get; init; } = default!;
    }
}
