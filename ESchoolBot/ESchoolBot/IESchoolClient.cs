
namespace ESchoolBot
{
    public interface IESchoolClient
    {
        Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken);
    }
}