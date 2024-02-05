using Microsoft.Data.Sqlite;

namespace ESchoolBot
{
    public class DatabaseAccessor : IDatabaseAccessor
    {
        private readonly string connectionString;

        public DatabaseAccessor(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public SqliteConnection CreateConnection()
        {
            var connection = new SqliteConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
