using Microsoft.Data.Sqlite;

namespace ESchoolBot
{
    public interface IDatabaseAccessor
    {
        SqliteConnection CreateConnection();
    }
}