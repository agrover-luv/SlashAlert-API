using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace SlashAlert.Api.Services
{
    public class CsvService : ICsvService
    {
        private readonly string _basePath;

        public CsvService()
        {
                // Try to locate the 'Database' folder by walking up parent directories.
                // This is more robust than assuming a fixed relative path since the
                // app may be started from different working directories.
                var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                DirectoryInfo? found = null;
                var maxDepth = 6; // avoid infinite loops
                for (int i = 0; i < maxDepth && dir != null; i++)
                {
                    var candidate = Path.Combine(dir.FullName, "Database");
                    if (Directory.Exists(candidate))
                    {
                        found = new DirectoryInfo(candidate);
                        break;
                    }
                    dir = dir.Parent;
                }

                if (found != null)
                {
                    _basePath = found.FullName;
                }
                else
                {
                    // Fallback to the original heuristic if not found
                    _basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Database"));
                }
        }

        public IEnumerable<Dictionary<string, string>> ReadCsv(string relativePath)
        {
            // Try several candidate locations to be robust when the app is started from
            // different working directories or CI environments.
            var candidates = new List<string>();
            candidates.Add(Path.GetFullPath(Path.Combine(_basePath, relativePath)));
            // relative to current directory upwards
            candidates.Add(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Database", relativePath)));
            // relative to AppContext base directory (where the DLL runs)
            candidates.Add(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Database", relativePath)));
            // fallback to known repo path (useful for local dev)
            candidates.Add(Path.GetFullPath(Path.Combine("/Users/abhisheksrivastava/WorkingDirectory/POCs/SlashAlert-Project/SlashAlert/Database", relativePath)));

            string? fullPath = candidates.FirstOrDefault(p => File.Exists(p));
            if (fullPath == null)
            {
                // If no candidate found, log to stdout for debugging and return empty
                System.Console.WriteLine("CsvService: Could not find CSV file. Tried:");
                foreach (var c in candidates) System.Console.WriteLine(c);
                return Enumerable.Empty<Dictionary<string, string>>();
            }

            // Diagnostic logging to help debug CSV parsing issues in local dev
            try
            {
                System.Console.WriteLine($"CsvService: Using CSV file: {fullPath}");
                var previewLines = File.ReadLines(fullPath).Take(2);
                System.Console.WriteLine("CsvService: File preview:");
                foreach (var ln in previewLines) System.Console.WriteLine(ln);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"CsvService: Failed to preview file {fullPath}: {ex.Message}");
            }

            using var reader = new StreamReader(fullPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                BadDataFound = null,
                HeaderValidated = null,
            });

            csv.Read();
            csv.ReadHeader();
            var header = csv.HeaderRecord ?? System.Array.Empty<string>();

            var results = new List<Dictionary<string, string>>();

            if (header.Length == 0)
                return results;

            while (csv.Read())
            {
                var dict = new Dictionary<string, string>();
                foreach (var h in header)
                {
                    // ensure we never assign null to the dictionary values
                    dict[h] = csv.GetField(h) ?? string.Empty;
                }
                results.Add(dict);
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string relativePath) where T : class, new()
        {
            return await Task.Run(() =>
            {
                var csvData = ReadCsv(relativePath);
                var results = new List<T>();

                foreach (var row in csvData)
                {
                    var obj = new T();
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in properties)
                    {
                        if (row.TryGetValue(property.Name, out var value) && !string.IsNullOrEmpty(value))
                        {
                            try
                            {
                                var convertedValue = ConvertToType(value, property.PropertyType);
                                property.SetValue(obj, convertedValue);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Warning: Failed to set property {property.Name} with value '{value}': {ex.Message}");
                            }
                        }
                    }
                    results.Add(obj);
                }

                return results.AsEnumerable();
            });
        }

        private static object? ConvertToType(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // Handle Guid
            if (underlyingType == typeof(Guid))
            {
                return Guid.TryParse(value, out var guid) ? guid : Guid.Empty;
            }

            // Handle DateTime
            if (underlyingType == typeof(DateTime))
            {
                return DateTime.TryParse(value, out var dateTime) ? dateTime : DateTime.MinValue;
            }

            // Handle decimal
            if (underlyingType == typeof(decimal))
            {
                return decimal.TryParse(value, out var decimalValue) ? decimalValue : 0m;
            }

            // Handle double
            if (underlyingType == typeof(double))
            {
                return double.TryParse(value, out var doubleValue) ? doubleValue : 0d;
            }

            // Handle int
            if (underlyingType == typeof(int))
            {
                return int.TryParse(value, out var intValue) ? intValue : 0;
            }

            // Handle bool
            if (underlyingType == typeof(bool))
            {
                return bool.TryParse(value, out var boolValue) ? boolValue : false;
            }

            // Use TypeConverter for other types
            var converter = TypeDescriptor.GetConverter(underlyingType);
            if (converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFromString(value);
            }

            // Default to string
            return value;
        }
    }
}
