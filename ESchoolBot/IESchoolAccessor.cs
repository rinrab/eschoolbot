
namespace ESchoolBot
{
    public interface IESchoolAccessor
    {
        Task<DiaryPeriodResponse.DiaryPeriod[]> GetDiariesAsync(DatabaseClient.User user, CancellationToken cancellationToken);
    }
}