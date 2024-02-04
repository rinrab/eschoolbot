
namespace ESchoolBot
{
    public interface IClient
    {
        Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken);
    }
}