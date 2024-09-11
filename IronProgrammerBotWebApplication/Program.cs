using StreamingCourses_Contracts.Abstractions;
using IRON_Infrastructure_Common;
using IronProgrammerBotWebApplication.Controllers;
using IronProgrammerBotWebApplication.Services;
using IronProgrammerBotWebApplication.Settings;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace IronProgrammerBotWebApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
            var botConfiguration = botConfigurationSection.Get<BotConfiguration>()!;
            var firebaseConfig = builder.Configuration.GetSection(Settings.Firebase.Name);
            var connection = builder.Configuration!.GetConnectionString("dbConnection")!;
            ContainerConfigurator.Configure(builder.Services, botConfiguration.BotToken, firebaseConfig, connection);
            builder.Services.AddScoped<IUpdateHandlers, UpdateHandlers>();
            builder.Services.AddHostedService<ConfigureWebhook>();
            builder.Services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
            var app = builder.Build();

            await ContainerConfigurator.Initialize(app.Services);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "/swagger/{documentName}/swagger.json";
                });
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bot"));
            }
            app.MapBotWebhookRoute<BotController>(route: botConfiguration!.Route);
            app.MapControllers();

            app.Run();
        }
    }
}
