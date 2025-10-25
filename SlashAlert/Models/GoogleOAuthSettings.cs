namespace SlashAlert.Models;

public class GoogleOAuthSettings
{
    public const string SectionName = "GoogleOAuth";
    
    public string ClientId { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public List<string> ValidAudiences { get; set; } = new();
    public List<string> ValidIssuers { get; set; } = new();
    public bool DisableAudienceValidation { get; set; } = false;
}