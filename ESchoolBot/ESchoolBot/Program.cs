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

            builder.ConfigureServices((context, services) =>
            {
                services.Configure<Config>(context.Configuration.GetSection(Config.SectionName));

                services.AddSingleton<IDatabaseAccessor, DatabaseAccessor>(serviceProvider =>
                    {
                        Config config = serviceProvider.GetService<IOptions<Config>>()!.Value;
                        return new DatabaseAccessor(config.ConnectionString);
                    });

                services.AddSingleton<IDatabaseClient, DatabaseClient>();

                services.AddHostedService<Service>();

                services.AddHttpClient("telegram_bot_client")
                    .AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
                    {
                        Config config = serviceProvider.GetService<IOptions<Config>>()!.Value;
                        TelegramBotClientOptions options = new TelegramBotClientOptions(config.BotToken);
                        return new TelegramBotClient(options, httpClient);
                    });
                
                services.AddHttpClient("eschool_client")
                    .AddTypedClient<IClient>((httpClient, serviceProvider) =>
                    {
                        return new Client(httpClient);
                    });
            });

            IHost app = builder.Build();

            app.Run();
        }
    }
}