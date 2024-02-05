namespace ESchoolBot
{
    public interface IDatabaseClient
    {
        void InsertUser(long chatId, string username, string password, string sessionId, int userId, int periodId);
        void UpdateProcessedDiaries(long chatId, int processedDiaries);
        void UpdateSessionId(long chatId, string sessionId);
        List<DatabaseClient.User> ListUsers();
    }
}
