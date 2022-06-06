using Api.Models;

namespace Api.Data
{
    public interface IRepository : IDisposable
    {
        Task InvalidateBoth(string token, User user);
        Task<bool> IsTokenValidAsync(string token);
        Task<Token> InvalidateTokenAsync(string token);
        Task<Token> RevalidateTokenAsync(string token);
        Task<bool> IsUserValidAsync(string name);
        Task<User> InvalidateUserAsync(User user);
        Task<User> RevalidateUserAsync(User user);
    }
}
