
namespace ESchoolBot
{
    public interface IESchoolAccessor
    {
        Task<FetchData> GetDiariesAsync(DatabaseClient.User user, CancellationToken cancellationToken);
    }
}