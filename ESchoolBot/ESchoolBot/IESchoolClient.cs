
namespace ESchoolBot
{
    public interface IESchoolClient
    {
        Task<string> LoginAsync(string email, string passwordHash, CancellationToken cancellationToken);
    }
}