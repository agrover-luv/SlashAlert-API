using UserModel = SlashAlert.Models.User;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<UserModel>
    {
        Task<UserModel?> GetByEmailAsync(string email);
        Task<UserModel?> GetBySubAsync(string sub);
        Task<IEnumerable<UserModel>> GetByProviderAsync(string provider);
        Task<IEnumerable<UserModel>> GetActiveUsersAsync();
        Task<IEnumerable<UserModel>> GetRecentLoginsAsync(int days = 30);
        Task<UserModel?> GetByPartitionKeyAsync(string id, string partitionKey);
    }
}