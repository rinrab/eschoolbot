using System.Text.Json;
using System.Text.Json.Serialization;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ESchoolBot
{
    public class Service : BackgroundService
    {
        private readonly ILogger logger;
        private readonly ITelegramBotClient botClient;
        private readonly IESchoolClient client;
        private readonly IDatabaseClient databaseClient;

        public Service(ILogger<Service> logger, ITelegramBotClient botClient, IESchoolClient client, IDatabaseClient databaseClient)
        {
            this.logger = logger;
            this.botClient = botClient;
            this.client = client;
            this.databaseClient = databaseClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Bot starting receiving events");

            await botClient.ReceiveAsync(
                HandleUpdateAsync,
                HandlePollingErrorAsync,
                null, stoppingToken
            );

            logger.LogInformation("Bot stopped receiving events");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            try
            {
                Message? message = update.Message;
                if (message != null)
                {
                    long chatId = message.Chat.Id;
                    string? messageText = message.Text;

                    if (messageText != null)
                    {
                        try
                        {
                            if (messageText == "/start")
                            {
                                await ProcessStartMessageAsync(chatId, cancellationToken);
                            }
                            else if (messageText == "/off")
                            {
                                await ProcessDisableMessageAsync(chatId, cancellationToken);
                            }
                            else
                            {
                                await SendHelpMessageAsync(chatId, cancellationToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error while processing message: '{messageText}'.", ex);
                        }

                        logger.LogInformation("Successfully processed message: '{messageText}' from {userId}", messageText, chatId);
                    }
                    else if (message.WebAppData != null)
                    {
                        await ProcessWebAppDataMessageAsync(chatId, message.WebAppData, cancellationToken);
                    }
                    else
                    {
                        await SendHelpMessageAsync(chatId, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while handling update:");
            }
        }

        private async Task SendHelpMessageAsync(long chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(chatId, Formatter.HelpMessage,
                                                 replyMarkup: new ReplyKeyboardRemove(),
                                                 parseMode: ParseMode.Markdown,
                                                 disableWebPagePreview: true,
                                                 cancellationToken: cancellationToken);
        }

        private static IReplyMarkup CreateLoginMarkup()
        {
            KeyboardButton button = KeyboardButton.WithWebApp(Formatter.LoginButtonText, new WebAppInfo
            {
                Url = "https://rinrab.github.io/eschoolbot/login.html"
            });

            return new ReplyKeyboardMarkup(button)
            {
                ResizeKeyboard = true,
                InputFieldPlaceholder = Formatter.LoginPlaceholder
            };
        }

        private async Task ProcessStartMessageAsync(long chatId, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(chatId, Formatter.StartMessage,
                                                 replyMarkup: CreateLoginMarkup(),
                                                 cancellationToken: cancellationToken);
        }

        private async Task ProcessDisableMessageAsync(long chatId, CancellationToken cancellationToken)
        {
            databaseClient.DisableUser(chatId);

            await botClient.SendTextMessageAsync(chatId, Formatter.BotDisabled,
                                                 cancellationToken: cancellationToken);
        }

        private async Task ProcessWebAppDataMessageAsync(long chatId, WebAppData data, CancellationToken cancellationToken)
        {
            LoginData? parsed = JsonSerializer.Deserialize<LoginData>(data.Data);

            if (parsed != null)
            {
                try
                {
                    string passwordHash = ESchoolClient.ComputeHash(parsed.Password);

                    string sessionId = await client.LoginAsync(parsed.Email, passwordHash, cancellationToken);

                    databaseClient.InsertUser(chatId, parsed.Email, passwordHash, sessionId);

                    await botClient.SendTextMessageAsync(chatId, Formatter.PostLogin,
                                                         replyMarkup: new ReplyKeyboardRemove(),
                                                         cancellationToken: cancellationToken);
                }
                catch (LoginException)
                {
                    await botClient.SendTextMessageAsync(chatId, Formatter.IncorrectLoginOrPassword,
                                                         replyMarkup: CreateLoginMarkup(),
                                                         cancellationToken: cancellationToken);
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId, "Cannot parse message from client.",
                                                     replyMarkup: CreateLoginMarkup(),
                                                     cancellationToken: cancellationToken);
            }
        }

        private async Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException &&
                apiRequestException.Parameters != null &&
                apiRequestException.Parameters.RetryAfter != null)
            {
                int retryAfter = apiRequestException.Parameters.RetryAfter.Value;
                logger.LogWarning(exception, "Too many requests. Retry after: {retryAfter} seconds.", retryAfter);
                await Task.Delay(retryAfter * 1000, cancellationToken);
            }
            else
            {
                logger.LogError(exception, "Telegram polling error");

                await Task.Delay(5000, cancellationToken);
            }
        }


        private class LoginData
        {
            [JsonPropertyName("email")]
            public required string Email { get; set; }

            [JsonPropertyName("password")]
            public required string Password { get; set; }
        }
    }
}