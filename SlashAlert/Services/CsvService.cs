using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    }
}
