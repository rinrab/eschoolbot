using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ESchoolBot
{
    public class NotificationService : BackgroundService
    {
        private readonly IDatabaseClient databaseClient;
        private readonly ILogger logger;
        private readonly ITelegramBotClient botClient;
        private readonly IESchoolAccessor eschoolAccessor;
        private readonly int fetchDelay;

        public NotificationService(IDatabaseClient databaseClient,
                                   ILogger<NotificationService> logger,
                                   ITelegramBotClient botClient,
                                   IESchoolAccessor eschoolAccessor,
                                   IOptions<Config> options)
        {
            this.databaseClient = databaseClient;
            this.logger = logger;
            this.botClient = botClient;
            this.eschoolAccessor = eschoolAccessor;

            fetchDelay = options.Value.FetchDelay;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Notification service started on every {fetchDelay} seconds.", fetchDelay);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (Formatter.GetDate().TimeOfDay.Hours > 7)
                {
                    foreach (DatabaseClient.User user in databaseClient.ListUsers())
                    {
                        if (user.IsEnabled)
                        {
                            try
                            {
                                logger.LogInformation("Fetching user {userId}", user.ChatId);
                                await FetchUser(user, stoppingToken);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Error while processing fetch for user {userId}", user.ChatId);
                            }
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(fetchDelay), stoppingToken);
            }

            logger.LogInformation("Notification service stopped");
        }

        private async Task FetchUser(DatabaseClient.User user, CancellationToken stoppingToken)
        {
            try
            {
                DiaryPeriod[] diaries = await eschoolAccessor.GetDiariesAsync(user, stoppingToken);

                List<DiaryPeriod> filteredDiaries = [];

                foreach (DiaryPeriod diary in diaries)
                {
                    if (diary.MarkDate.HasValue && diary.MarkDate.Value > user.ProcessedDate)
                    {
                        filteredDiaries.Add(diary);
                    }
                }

                filteredDiaries.Sort(DiaryComparer);

                foreach (DiaryPeriod diary in filteredDiaries)
                {
                    await botClient.SendTextMessageAsync(user.ChatId,
                                                         Formatter.FormatNewDiaryMessage(diary),
                                                         parseMode: ParseMode.Html,
                                                         cancellationToken: stoppingToken);

                    databaseClient.UpdateProcessedDate(user.ChatId, diary.MarkDate!.Value);
                }
            }
            catch (LoginException)
            {
            }
        }

        private static int DiaryComparer(DiaryPeriod a, DiaryPeriod b)
        {
            return DateTime.Compare(a.MarkDate!.Value, b.MarkDate!.Value);
        }
    }
}
