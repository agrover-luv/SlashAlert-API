using SlashAlert.Api.Services;
using SlashAlert.Repositories.Interfaces;
using UserModel = SlashAlert.Models.User;

namespace SlashAlert.Repositories.Adapters.Csv
{
    public class CsvUserRepository : BaseCsvRepository<UserModel>, IUserRepository
    {
        public CsvUserRepository(ICsvService csvService) : base(csvService, "User_export.csv") { }

        public async Task<UserModel?> GetByEmailAsync(string email)
        {
            var users = await GetAllAsync();
            return users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<UserModel?> GetBySubAsync(string sub)
        {
            var users = await GetAllAsync();
            return users.FirstOrDefault(u => u.Sub == sub);
        }

        public async Task<IEnumerable<UserModel>> GetByProviderAsync(string provider)
        {
            var users = await GetAllAsync();
            return users.Where(u => u.Provider.Equals(provider, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<UserModel>> GetActiveUsersAsync()
        {
            var users = await GetAllAsync();
            return users.Where(u => u.IsActive);
        }

        public async Task<IEnumerable<UserModel>> GetRecentLoginsAsync(int days = 30)
        {
            var users = await GetAllAsync();
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            return users.Where(u => u.LastLogin.HasValue && u.LastLogin >= cutoffDate);
        }

        public async Task<UserModel?> GetByPartitionKeyAsync(string id, string partitionKey)
        {
            var users = await GetAllAsync();
            return users.FirstOrDefault(u => u.Id == id && u.PartitionKey == partitionKey);
        }
    }
}