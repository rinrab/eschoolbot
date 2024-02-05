using Telegram.Bot;

namespace ESchoolBot
{
    public class NotificationService : BackgroundService
    {
        private readonly IDatabaseClient databaseClient;
        private readonly ILogger logger;
        private readonly ITelegramBotClient botClient;
        private readonly IESchoolClient eschoolClient;
        private readonly int fetchDelay = 10;

        public NotificationService(IDatabaseClient databaseClient,
                                   ILogger<NotificationService> logger,
                                   ITelegramBotClient botClient,
                                   IESchoolClient eschoolClient)
        {
            this.databaseClient = databaseClient;
            this.logger = logger;
            this.botClient = botClient;
            this.eschoolClient = eschoolClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Notification service started on every {fetchDelay} seconds.", fetchDelay);

            while (!stoppingToken.IsCancellationRequested)
            {
                List<DatabaseClient.User> users = databaseClient.ListUsers();

                foreach (DatabaseClient.User user in users)
                {
                    try
                    {
                        await FetchUser(user, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error while processing fetch for user {userId}", user.ChatId);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(fetchDelay), stoppingToken);
            }
        }

        private async Task FetchUser(DatabaseClient.User user, CancellationToken stoppingToken)
        {
            DiaryPeriodResponse diariesResponse = await InvokeESchoolClientAsync(user,
                async (sessionId, cancellationToken) =>
                {
                    return await eschoolClient.GetDiaryPeriodAsync(user.SessionId, user.UserId, user.PeriodId);
                }, stoppingToken);

            DiaryPeriodResponse.DiaryPeriod[] diaries = diariesResponse.Result;

            if (user.ProcessedDiaries == -1)
            {
                databaseClient.UpdateProcessedDiaries(user.ChatId, diaries.Length);
            }
            else
            {
                for (int i = diaries.Length - 1, sentMessages = 0; i >= user.ProcessedDiaries; i--)
                {
                    logger.LogInformation("{test}", diaries[i]);
                    if (diaries[i].Subject != null)
                    {
                        await botClient.SendTextMessageAsync(user.ChatId, Formatter.FormatNewDiaryMessage(diaries[i]), cancellationToken: stoppingToken);
                        sentMessages++;
                    }

                    if (sentMessages == 3)
                    {
                        break;
                    }
                }
                databaseClient.UpdateProcessedDiaries(user.ChatId, diaries.Length);
            }
        }

        private delegate Task<T> InvokeESchoolClientAction<T>(string sessionId, CancellationToken cancellationToken);

        private async Task<T> InvokeESchoolClientAsync<T>(DatabaseClient.User user,
                                                          InvokeESchoolClientAction<T> action,
                                                          CancellationToken cancellationToken)
        {
            try
            {
                return await action(user.SessionId, cancellationToken);
            }
            catch (LoginException)
            {
                logger.LogInformation("Updating SessionId for user {username}", user.Username);

                string newToken = await eschoolClient.LoginAsync(user.Username, user.Password, cancellationToken);

                databaseClient.UpdateSessionId(user.ChatId, newToken);

                try
                {
                    return await action(newToken, cancellationToken);
                }
                catch (LoginException)
                {
                    await botClient.SendTextMessageAsync(user.ChatId,
                                                         Formatter.LoginRequired,
                                                         cancellationToken: cancellationToken);
                    throw;
                }
            }
        }
    }
}
