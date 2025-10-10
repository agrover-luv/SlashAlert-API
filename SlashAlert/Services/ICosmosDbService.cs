using SlashAlert.Models;
using UserModel = SlashAlert.Models.User;

namespace SlashAlert.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<UserModel>> GetAllUsersAsync();
        Task<UserModel?> GetUserByIdAsync(string id, string partitionKey);
        Task<UserModel?> GetUserBySubAsync(string sub);
        Task<UserModel?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserModel>> GetUsersByProviderAsync(string provider);
        Task<IEnumerable<UserModel>> GetActiveUsersAsync();
        Task<IEnumerable<UserModel>> GetRecentLoginsAsync(int days = 30);
    }
}