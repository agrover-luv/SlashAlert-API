namespace SlashAlert.Modelsnamespace SlashAlert.Models

{{

    public class CosmosDbSettings    public class CosmosDbSettings

    {    {

        public string EndpointUri { get; set; } = string.Empty;        public string EndpointUri { get; set; } = string.Empty;

        public string PrimaryKey { get; set; } = string.Empty;        public string PrimaryKey { get; set; } = string.Empty;

        public string DatabaseName { get; set; } = string.Empty;        public string DatabaseName { get; set; } = string.Empty;

        public string UsersContainerName { get; set; } = string.Empty;        public string UsersContainerName { get; set; } = string.Empty;

        public string AlertsContainerName { get; set; } = string.Empty;        public string AlertsContainerName { get; set; } = string.Empty;

                

        // Keep for backward compatibility        // Keep for backward compatibility

        public string ContainerName { get; set; } = string.Empty;        public string ContainerName { get; set; } = string.Empty;

    }    }

}}