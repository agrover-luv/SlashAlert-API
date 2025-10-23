namespace SlashAlert.Models
{
    public class DatabaseSettings
    {
        public const string SectionName = "Database";
        
        public string Provider { get; set; } = "CSV"; // CSV, CosmosDB, SQL
        public CsvSettings? Csv { get; set; }
        public CosmosDbSettings? CosmosDb { get; set; }
        public SqlSettings? Sql { get; set; }
    }

    public class CsvSettings
    {
        public string DatabasePath { get; set; } = "Database";
    }

    public class SqlSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string Provider { get; set; } = "SqlServer"; // SqlServer, PostgreSQL, MySQL
    }
}