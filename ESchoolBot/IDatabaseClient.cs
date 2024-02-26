namespace ESchoolBot
{
    public interface IDatabaseClient
    {
        void InsertUser(DatabaseClient.User user);
        List<DatabaseClient.User> ListUsers();

        void UpdateSessionId(long chatId, string sessionId);
        void UpdateProcessedDate(long userId, DateTime date);
        void DisableUser(long chatId);
        int GetVersion();
    }
}
