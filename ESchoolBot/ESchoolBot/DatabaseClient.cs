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
                            session_id  INTEGER,
                            UNIQUE(chat_id)
                        );

                        INSERT INTO schema (version) VALUES (1);
                        """;

                        command.ExecuteNonQuery();
                    }
                }

                if (GetVersion() < 2)
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText =
                        """
                        ALTER TABLE users ADD COLUMN user_id INTEGER;
                        ALTER TABLE users ADD COLUMN period_id INTEGER;

                        UPDATE schema SET version=2;
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

        public void InsertUser(long chatId, string username, string password, string sessionId, int userId, int periodId)
        {
            using (SqliteConnection connection = databaseAccessor.CreateConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    """
                    INSERT INTO users (chat_id, username, password, session_id, user_id, period_id)
                        VALUES ($chat_id, $username, $password, $session_id, $user_id, $period_id);
                    """;
                command.Parameters.AddWithValue("chat_id", chatId);
                command.Parameters.AddWithValue("username", username);
                command.Parameters.AddWithValue("password", password);
                command.Parameters.AddWithValue("session_id", sessionId);
                command.Parameters.AddWithValue("user_id", userId);
                command.Parameters.AddWithValue("period_id", periodId);

                command.ExecuteNonQuery();
            }
        }
        
        public List<User> ListUsers()
        {
            using (SqliteConnection connection = databaseAccessor.CreateConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    """
                    SELECT chat_id, username, password, session_id, user_id, period_id FROM users;
                    """;

                List<User> users = new List<User>();

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            ChatId = reader.GetInt64(0),
                            Username = reader.GetString(1),
                            Password = reader.GetString(2),
                            SessionId = reader.GetString(3),
                            UserId = reader.GetInt32(4),
                            PeriodId = reader.GetInt32(5),
                        });
                    }
                }

                return users;
            }
        }

        public class User
        {
            public long ChatId { get; set; }
            public required string Username { get; set; }
            public required string Password { get; set; }
            public required string SessionId { get; set; }
            public required int UserId { get; set; }
            public required int PeriodId { get; set; }
        }
    }
}
