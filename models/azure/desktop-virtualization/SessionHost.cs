namespace SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

public class SessionHost : ISessionHost
{
    public SessionHost() {}

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("resourceId")]
    public string? ResourceId { get; set; }

    [JsonPropertyName("properties")]
    public SessionHostProperties? Properties { get; set; }
}