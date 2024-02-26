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

                if (GetVersion() < 3)
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText =
                        """
                        ALTER TABLE users ADD COLUMN processed_diaries INTEGER;

                        UPDATE schema SET version=3;
                        """;

                        command.ExecuteNonQuery();
                    }
                }

                if (GetVersion() < 4)
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText =
                        """
                        ALTER TABLE users ADD COLUMN processed_date STRING;

                        UPDATE schema SET version=4;
                        """;

                        command.ExecuteNonQuery();
                    }
                }

                if (GetVersion() < 5)
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText =
                        """
                        ALTER TABLE users DROP COLUMN user_id;
                        ALTER TABLE users DROP COLUMN period_id;
                        ALTER TABLE users DROP COLUMN processed_diaries;

                        UPDATE schema SET version=5;
                        """;

                        command.ExecuteNonQuery();
                    }
                }


                if (GetVersion() < 6)
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText =
                        """
                        ALTER TABLE users ADD COLUMN is_enabled BOOL;

                        UPDATE schema SET version=6;
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

        public void InsertUser(User user)
        {
            using (SqliteConnection connection = databaseAccessor.CreateConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    """
                    INSERT OR REPLACE INTO
                        users (chat_id, username, password, session_id, processed_date, is_enabled)
                        VALUES ($chat_id, $username, $password, $session_id, $processed_date, $is_enabled);
                    """;
                command.Parameters.AddWithValue("chat_id", user.ChatId);
                command.Parameters.AddWithValue("username", user.Username);
                command.Parameters.AddWithValue("password", user.Password);
                command.Parameters.AddWithValue("session_id", user.SessionId);
                command.Parameters.AddWithValue("processed_date", user.ProcessedDate);
                command.Parameters.AddWithValue("is_enabled", user.IsEnabled);

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
                    SELECT chat_id,
                           username,
                           password,
                           session_id,
                           processed_date,
                           is_enabled
                    FROM users;
                    """;

                List<User> users = [];

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
                            ProcessedDate = reader.GetDateTime(4),
                            IsEnabled = reader.GetBoolean(5)
                        });
                    }
                }

                return users;
            }
        }

        public void UpdateProcessedDate(long chatId, DateTime date)
        {
            using (SqliteConnection connection = databaseAccessor.CreateConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE users SET processed_date=$processed_date WHERE chat_id=$chat_id;";
                command.Parameters.AddWithValue("processed_date", date);
                command.Parameters.AddWithValue("chat_id", chatId);

                command.ExecuteNonQuery();
            }
        }

        public void UpdateSessionId(long chatId, string sessionId)
        {
            using (SqliteConnection connection = databaseAccessor.CreateConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE users SET session_id=$session_id WHERE chat_id=$chat_id;";
                command.Parameters.AddWithValue("session_id", sessionId);
                command.Parameters.AddWithValue("chat_id", chatId);

                command.ExecuteNonQuery();
            }
        }

        public void DisableUser(long chatId)
        {
            using (SqliteConnection connection = databaseAccessor.CreateConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE users SET is_enabled=FALSE WHERE chat_id=$chat_id;";
                command.Parameters.AddWithValue("chat_id", chatId);

                command.ExecuteNonQuery();
            }
        }

        public class User
        {
            public long ChatId { get; set; }
            public required string Username { get; set; }
            public required string Password { get; set; }
            public required string SessionId { get; set; }
            public required DateTime ProcessedDate { get; set; }
            public required bool IsEnabled { get; set; }
        }
    }
}
