using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace ESchoolBot
{
    public class DatabaseAccessor : IDatabaseAccessor
    {
        private readonly string connectionString;

        public DatabaseAccessor(IOptions<Config> options)
        {
            connectionString = options.Value.ConnectionString;
        }

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
