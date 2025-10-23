using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using System.Reflection;
using System.Text.Json;

namespace SlashAlert.Repositories.Adapters.Csv
{
    public abstract class BaseCsvRepository<T> : IRepository<T> where T : BaseEntity, new()
    {
        protected readonly ICsvService _csvService;
        protected readonly string _fileName;

        protected BaseCsvRepository(ICsvService csvService, string fileName)
        {
            _csvService = csvService;
            _fileName = fileName;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(GetAllEntities());
        }

        public virtual async Task<T?> GetByIdAsync(string id)
        {
            var entities = GetAllEntities();
            return await Task.FromResult(entities.FirstOrDefault(e => e.Id == id));
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            // For CSV, we'll simulate creation by returning the entity with a generated ID
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString("N")[..24]; // MongoDB-like ID
            }
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = DateTime.UtcNow;

            // In a real implementation, you'd write back to CSV
            // For this POC, we'll just return the entity
            return await Task.FromResult(entity);
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            
            // In a real implementation, you'd update the CSV file
            // For this POC, we'll just return the entity
            return await Task.FromResult(entity);
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            // In a real implementation, you'd remove from CSV file
            // For this POC, we'll simulate success
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public virtual async Task<bool> ExistsAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public virtual async Task<int> CountAsync()
        {
            var entities = GetAllEntities();
            return await Task.FromResult(entities.Count());
        }

        protected IEnumerable<T> GetAllEntities()
        {
            var csvData = _csvService.ReadCsv(_fileName);
            return csvData.Select(ConvertDictionaryToEntity).Where(e => e != null).Cast<T>();
        }

        protected virtual T ConvertDictionaryToEntity(Dictionary<string, string> csvRow)
        {
            var entity = new T();
            var properties = typeof(T).GetProperties();

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

        private string GetJsonPropertyName(PropertyInfo property)
        {
            var jsonPropertyAttribute = property.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>();
            return jsonPropertyAttribute?.Name ?? property.Name;
        }

        private void SetPropertyValue(T entity, PropertyInfo property, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

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
                        // Handle string representations like "true"/"false" or "1"/"0"
                        property.SetValue(entity, value.ToLower() == "true" || value == "1");
                    }
                }
                else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        property.SetValue(entity, intValue);
                    }
                }
                else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                {
                    if (decimal.TryParse(value, out var decimalValue))
                    {
                        property.SetValue(entity, decimalValue);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but continue processing
                System.Console.WriteLine($"Error setting property {property.Name} with value '{value}': {ex.Message}");
            }
        }
    }
}