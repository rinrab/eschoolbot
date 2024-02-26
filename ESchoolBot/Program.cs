using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace ESchoolBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IHostBuilder builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureHostConfiguration(builder =>
            {
                builder.AddUserSecrets(typeof(Program).Assembly);
            });

            builder.UseWindowsService(options =>
            {
                options.ServiceName = "eschoolbot";
            });

            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddEventLog(new EventLogSettings()
                {
                    LogName = "eschoolbot",
                    SourceName = "eschoolbot",
                });
            });

            builder.ConfigureServices((context, services) =>
            {
                services.Configure<Config>(context.Configuration.GetSection(Config.SectionName));

                services.AddSingleton<IDatabaseAccessor, DatabaseAccessor>();
                services.AddSingleton<IDatabaseClient, DatabaseClient>();
                services.AddSingleton<IESchoolAccessor, ESchoolAccessor>();
                services.AddHostedService<TelegramService>();
                services.AddHostedService<NotificationService>();

                services.AddHttpClient("telegram_bot_client")
                    .AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
                    {
                        Config config = serviceProvider.GetService<IOptions<Config>>()!.Value;
                        TelegramBotClientOptions options = new TelegramBotClientOptions(config.BotToken);
                        return new TelegramBotClient(options, httpClient);
                    });

                services.AddHttpClient("eschool_client")
                    .AddTypedClient<IESchoolClient>((httpClient, serviceProvider) =>
                    {
                        return new ESchoolClient(httpClient);
                    });
            });

            IHost app = builder.Build();

            app.Run();
        }
    }
}