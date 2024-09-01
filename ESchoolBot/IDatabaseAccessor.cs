using Microsoft.Data.Sqlite;

namespace ESchoolBot
{
    public interface IDatabaseAccessor
    {
        string ConnectionString { get; }
        SqliteConnection CreateConnection();
    }
}