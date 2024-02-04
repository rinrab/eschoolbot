using Microsoft.Data.Sqlite;

namespace ESchoolBot
{
    public class DatabaseClient : IDatabaseClient
    {
        private readonly IDatabaseAccessor databaseAccessor;

        public DatabaseClient(IDatabaseAccessor databaseAccessor)
        {
            this.databaseAccessor = databaseAccessor;

            Upgrade();
        }

        private void Upgrade()
        {
            using (var connection = databaseAccessor.CreateConnection())
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                    """
                    CREATE TABLE IF NOT EXISTS schema (
                        version         INTEGER
                    );
                    """;

                    command.ExecuteNonQuery();
                }

                if (GetVersion() < 1)
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText =
                        """
                        CREATE TABLE users (
                            chat_id     INTEGER,
                            username    INTEGER,
                            password    TEXT,
                            session_id  INTEGER
                        );

                        UPDATE schema SET version=1;
                        """;

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private int GetVersion()
        {
            using (SqliteConnection connection = databaseAccessor.CreateConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT version FROM schema";
                return (int?)(long?)command.ExecuteScalar() ?? 0;
            }
        }

        public void InsertUser(long chatId, string username, string password, string sessionId)
        {
            using (SqliteConnection connection = databaseAccessor.CreateConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    """
                    INSERT INTO users (chat_id, username, password, session_id)
                        VALUES ($chat_id, $username, $password, $session_id);
                    """;
                command.Parameters.AddWithValue("chat_id", chatId);
                command.Parameters.AddWithValue("username", username);
                command.Parameters.AddWithValue("password", password);
                command.Parameters.AddWithValue("session_id", sessionId);

                command.ExecuteNonQuery();
            }
        }
    }
}
