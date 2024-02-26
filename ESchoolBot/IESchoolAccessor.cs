
namespace ESchoolBot
{
    public interface IESchoolAccessor
    {
        Task<DiaryPeriod[]> GetDiariesAsync(DatabaseClient.User user, CancellationToken cancellationToken);
    }
}