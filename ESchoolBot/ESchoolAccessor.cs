
using Telegram.Bot;

namespace ESchoolBot
{
    public class ESchoolAccessor : IESchoolAccessor
    {
        private readonly IDatabaseClient databaseClient;
        private readonly IESchoolClient eschoolClient;
        private readonly ITelegramBotClient botClient;
        private readonly ILogger<ESchoolAccessor> logger;

        public ESchoolAccessor(IDatabaseClient databaseClient,
                               IESchoolClient eschoolClient,
                               ITelegramBotClient botClient,
                               ILogger<ESchoolAccessor> logger)
        {
            this.databaseClient = databaseClient;
            this.eschoolClient = eschoolClient;
            this.botClient = botClient;
            this.logger = logger;
        }

        public async Task<DiaryPeriod[]> GetDiariesAsync(DatabaseClient.User user, CancellationToken cancellationToken)
        {
            StateResponse state = await InvokeESchoolClientAsync(
                user,
                async (sessionId, cancellationToken) =>
                {
                    return await eschoolClient.GetStateAsync(sessionId);
                },
                cancellationToken);

            GroupsResponse groups = await InvokeESchoolClientAsync(
                user,
                async (sessionId, cancellationToken) =>
                {
                    return await eschoolClient.GetGroupsAsync(sessionId, state.UserId);
                },
                cancellationToken);

            PeriodsResponse periods = await InvokeESchoolClientAsync(
                user,
                async (sessionId, cancellationtoken) =>
                {
                    return await eschoolClient.GetPeriodsAsync(sessionId, groups.Last().GroupId);
                },
                cancellationToken);

            int periodId = periods.Items.First().Id;

            DiaryPeriodResponse diariesResponse = await InvokeESchoolClientAsync(
                user,
                async (sessionId, cancellationToken) =>
                {
                    return await eschoolClient.GetDiaryPeriodAsync(sessionId, state.UserId, periodId);
                },
                cancellationToken);

            return diariesResponse.Result;
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
                try
                {
                    logger.LogInformation("Updating SessionId for user {chatId}", user.ChatId);

                    string newToken = await eschoolClient.LoginAsync(user.Username, user.Password, cancellationToken);

                    logger.LogInformation(
                        """
                        Successfully updated session id for user {chatId}.
                        From: {oldSessionId}
                        To: {newSessionId}
                        """,
                        user.ChatId, user.SessionId, newToken);

                    databaseClient.UpdateSessionId(user.ChatId, newToken);

                    return await action(newToken, cancellationToken);
                }
                catch (LoginException)
                {
                    await botClient.SendTextMessageAsync(user.ChatId,
                                                         Formatter.LoginRequired,
                                                         cancellationToken: cancellationToken);

                    databaseClient.DisableUser(user.ChatId);

                    logger.LogWarning("Notifications for user {chatId} was disabled due to login error", user.ChatId);

                    throw;
                }
            }
        }
    }
}
