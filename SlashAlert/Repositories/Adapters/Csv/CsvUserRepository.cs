using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using System.Reflection;
using UserModel = SlashAlert.Models.User;

namespace SlashAlert.Repositories.Adapters.Csv
{
    public class CsvUserRepository : IUserRepository
    {
        protected readonly ICsvService _csvService;
        protected readonly string _fileName;

        public CsvUserRepository(ICsvService csvService) 
        { 
            _csvService = csvService;
            _fileName = "User_export.csv";
        }

        // IRepository<User> implementation - Users don't need email filtering
        public virtual async Task<IEnumerable<UserModel>> GetAllAsync(string userEmail)
        {
            // For users, we return all users (this might be restricted in real scenarios)
            return await Task.FromResult(GetAllUsers());
        }

        public virtual async Task<UserModel?> GetByIdAsync(string id, string userEmail)
        {
            var users = GetAllUsers();
            return await Task.FromResult(users.FirstOrDefault(e => e.Id == id));
        }

        public virtual async Task<UserModel> CreateAsync(UserModel entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString("N")[..24];
            }
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = DateTime.UtcNow;

            return await Task.FromResult(entity);
        }

        public virtual async Task<UserModel> UpdateAsync(UserModel entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            return await Task.FromResult(entity);
        }

        public virtual async Task<bool> DeleteAsync(string id, string userEmail)
        {
            var entity = await GetByIdAsync(id, userEmail);
            return entity != null;
        }

        public virtual async Task<bool> ExistsAsync(string id, string userEmail)
        {
            var entity = await GetByIdAsync(id, userEmail);
            return entity != null;
        }

        public virtual async Task<int> CountAsync(string userEmail)
        {
            var users = GetAllUsers();
            return await Task.FromResult(users.Count());
        }

        // IUserRepository specific methods
        public async Task<UserModel?> GetByEmailAsync(string email)
        {
            var users = GetAllUsers();
            return await Task.FromResult(users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<UserModel?> GetBySubAsync(string sub)
        {
            var users = GetAllUsers();
            return await Task.FromResult(users.FirstOrDefault(u => u.Sub == sub));
        }

        public async Task<IEnumerable<UserModel>> GetByProviderAsync(string provider)
        {
            var users = GetAllUsers();
            return await Task.FromResult(users.Where(u => u.Provider.Equals(provider, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<UserModel>> GetActiveUsersAsync()
        {
            var users = GetAllUsers();
            return await Task.FromResult(users.Where(u => u.IsActive));
        }

        public async Task<IEnumerable<UserModel>> GetRecentLoginsAsync(int days = 30)
        {
            var users = GetAllUsers();
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            return await Task.FromResult(users.Where(u => u.LastLogin.HasValue && u.LastLogin >= cutoffDate));
        }

        public async Task<UserModel?> GetByPartitionKeyAsync(string id, string partitionKey)
        {
            var users = GetAllUsers();
            return await Task.FromResult(users.FirstOrDefault(u => u.Id == id && u.PartitionKey == partitionKey));
        }

        private IEnumerable<UserModel> GetAllUsers()
        {
            var csvData = _csvService.ReadCsv(_fileName);
            return csvData.Select(ConvertDictionaryToUser).Where(e => e != null).Cast<UserModel>();
        }

        private UserModel ConvertDictionaryToUser(Dictionary<string, string> csvRow)
        {
            var entity = new UserModel();
            var properties = typeof(UserModel).GetProperties();

            foreach (var property in properties)
            {
                var jsonPropertyName = GetJsonPropertyName(property);
                if (csvRow.TryGetValue(jsonPropertyName, out var value))
                {
                    SetPropertyValue(entity, property, value);
                }
            }

            return entity;
        }

        private string GetJsonPropertyName(System.Reflection.PropertyInfo property)
        {
            var jsonPropertyAttribute = property.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>();
            return jsonPropertyAttribute?.Name ?? property.Name;
        }

        private void SetPropertyValue(UserModel entity, System.Reflection.PropertyInfo property, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            try
            {
                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(entity, value);
                }
                else if (property.PropertyType == typeof(DateTime?) || property.PropertyType == typeof(DateTime))
                {
                    if (DateTime.TryParse(value, out var dateValue))
                    {
                        property.SetValue(entity, dateValue);
                    }
                }
                else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                {
                    if (bool.TryParse(value, out var boolValue))
                    {
                        property.SetValue(entity, boolValue);
                    }
                    else
                    {
                        property.SetValue(entity, value.ToLower() == "true" || value == "1");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error setting property {property.Name} with value '{value}': {ex.Message}");
            }
        }
    }
}