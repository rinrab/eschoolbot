namespace ESchoolBot
{
    public interface IDatabaseClient
    {
        void InsertUser(long chatId, string username, string password, string sessionId, int userId, int periodId);
        void UpdateProcessedDiaries(long chatId, int processedDiaries);
        List<DatabaseClient.User> ListUsers();
    }
}
