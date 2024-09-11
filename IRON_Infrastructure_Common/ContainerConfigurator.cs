using Firebase.Database;
using StreamingCourses_Domain;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations;
using StreamingCourses_Implementations.Firebase;
using StreamingCourses_Implementations.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace IRON_Infrastructure_Common
{
    public static class ContainerConfigurator
    {
        public static void Configure(IServiceCollection services, string token, IConfigurationSection firebaseConfig, string connection)
        {
            services.AddHttpClient("telegram_bot_client")
                            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                            {
                                TelegramBotClientOptions options = new(token);
                                return new TelegramBotClient(options, httpClient);
                            });

            services.AddDbContextFactory<DatabaseContext>(options =>
            {
                options.UseSqlServer(connection);
            });

            var basePath = firebaseConfig.GetSection("BasePath").Value;
            var firebaseSecret = firebaseConfig.GetSection("Secret").Value;

            services.AddSingleton<IPagesFactory, PagesFactory>();
            services.AddSingleton<IFirebaseProvider, FirebaseProvider>();
            services.AddSingleton<FirebaseClient>(services =>
            {

                return new FirebaseClient(basePath, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(firebaseSecret),
                    JsonSerializerSettings = new JsonSerializerSettings()
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
                });
            });

            services.AddSingleton<IUserStateStorage, UserStateStorage>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICuratorService, CuratorService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IUpdateHandler, UpdateHandlers>();
            services.AddScoped<IResourcesService, ResourcesService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IWorkloadService, WorkloadService>();

            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name!.Contains("Implementations"));
            if (assembly == null)
                throw new ArgumentNullException("Нет сборки");

            assembly!
                .GetTypes()
                .Where(t => typeof(IPage).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList()
                .ForEach(t =>
                {
                    if(!services.Any(serviceDescriptor => serviceDescriptor.ServiceType == t))
                    {
                        services.AddScoped(t);
                    }
                });

            services.AddSingleton<IPagesFactory, PagesFactory>();
        }
        public static async Task Initialize(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var scopedServises = scope.ServiceProvider;
                using (DatabaseContext context = scopedServises.GetRequiredService<DatabaseContext>())
                {
                    await context.Database.MigrateAsync();
                    await IdentityInitializer.Initialize(context);
                }
            }
        }


    }
}
