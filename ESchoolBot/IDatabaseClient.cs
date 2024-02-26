namespace ESchoolBot
{
    public interface IDatabaseClient
    {
        void InsertUser(long chatId, string username, string password, string sessionId);
        void UpdateSessionId(long chatId, string sessionId);
        void UpdateProcessedDate(long userId, DateTime date);
        List<DatabaseClient.User> ListUsers();
    }
}
