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
                services.AddHostedService<Service>();

                services.AddHttpClient("telegram_bot_client")
                    .AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
                    {
                        TelegramBotClientOptions options = new TelegramBotClientOptions("6877687541:AAHp62IN5oeWn__1fJdJ6c3v5g40UzEBDsE");
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