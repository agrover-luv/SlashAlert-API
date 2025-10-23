using System.Collections.Generic;

namespace SlashAlert.Api.Services
{
    public interface ICsvService
    {
        IEnumerable<Dictionary<string, string>> ReadCsv(string relativePath);
    }
}
