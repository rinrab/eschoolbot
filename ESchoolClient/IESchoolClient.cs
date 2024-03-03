
using ESchoolClient;

namespace ESchoolClient
{
    public interface IESchoolClient
    {
        Task<string> LoginAsync(string email, string passwordHash, CancellationToken cancellationToken);
        Task<StateResponse> GetStateAsync(string sessionId);
        Task<DiaryPeriodResponse> GetDiaryPeriodAsync(string sessionId, int userId, int personId);
        Task<DiaryUnitsResponse> GetDiaryUnitsAsync(string sessionId, int userId, int periodId);
        Task<GroupsResponse> GetGroupsAsync(string sessionId, int userId);
        Task<PeriodsResponse> GetPeriodsAsync(string sessionId, int groupId);
    }
}