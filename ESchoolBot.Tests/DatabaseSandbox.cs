using Microsoft.Data.Sqlite;

namespace ESchoolBot.Tests
{
    public class DatabaseSandbox : Sandbox
    {
        public string DatabasePath { get; }

        public string ConnectionString { get; }

        public IDatabaseAccessor DatabaseAccessor { get; }

        public DatabaseSandbox()
        {
            DatabasePath = Path.Combine(RootPath, Guid.NewGuid().ToString() + ".db");

            ConnectionString = new SqliteConnectionStringBuilder
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                DataSource = DatabasePath
            }.ToString();

            DatabaseAccessor = new DatabaseAccessor(ConnectionString);
        }
    }
}
