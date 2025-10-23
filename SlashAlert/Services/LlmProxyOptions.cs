namespace SlashAlert.Api.Services
{
    public class LlmProxyOptions
    {
        // Primary provider: openai. Store the API key in environment or appsettings under LlmProxy:OpenAiApiKey
        public string? OpenAiApiKey { get; set; }
        // Optional fallback: HuggingFace
        public string? HuggingFaceApiKey { get; set; }
        public string? HuggingFaceModel { get; set; }
    }
}
