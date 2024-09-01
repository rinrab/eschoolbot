using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace ESchoolBot
{
    public class DatabaseAccessor : IDatabaseAccessor
    {
        public string ConnectionString { get; }

        public DatabaseAccessor(IOptions<Config> options)
        {
            ConnectionString = options.Value.ConnectionString;
        }

        public DatabaseAccessor(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public SqliteConnection CreateConnection()
        {
            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
