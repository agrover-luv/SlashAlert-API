using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.Sql
{
    public class SqlUserRepository : BaseSqlRepository<Models.User>, IUserRepository
    {
        public async Task<Models.User?> GetByEmailAsync(string email)
        {
            // TODO: Implement SQL query for user by email
            throw new NotImplementedException("SQL repository not yet implemented");
        }

        public async Task<Models.User?> GetBySubAsync(string sub)
        {
            // TODO: Implement SQL query for user by sub
            throw new NotImplementedException("SQL repository not yet implemented");
        }

        public async Task<IEnumerable<Models.User>> GetByProviderAsync(string provider)
        {
            // TODO: Implement SQL query for users by provider
            throw new NotImplementedException("SQL repository not yet implemented");
        }

        public async Task<IEnumerable<Models.User>> GetActiveUsersAsync()
        {
            // TODO: Implement SQL query for active users
            throw new NotImplementedException("SQL repository not yet implemented");
        }

        public async Task<IEnumerable<Models.User>> GetRecentLoginsAsync(int days = 30)
        {
            // TODO: Implement SQL query for recent logins
            throw new NotImplementedException("SQL repository not yet implemented");
        }

        public async Task<Models.User?> GetByPartitionKeyAsync(string id, string partitionKey)
        {
            // TODO: Implement SQL query for user by id and partition key
            throw new NotImplementedException("SQL repository not yet implemented");
        }
    }
}