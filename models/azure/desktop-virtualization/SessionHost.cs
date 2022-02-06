namespace SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

/// <summary>
/// Data pulled from the Azure API about an Azure Virtual Desktop session host.
/// 
/// <seealso href="https://docs.microsoft.com/en-us/rest/api/desktopvirtualization/session-hosts/get#sessionhost" />
/// </summary>
public class SessionHost : ISessionHost
{
    public SessionHost() {}

    /// <summary>
    /// The name of the session host.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The resource ID of the session host.
    /// </summary>
    [JsonPropertyName("resourceId")]
    public string? ResourceId { get; set; }

    [JsonPropertyName("properties")]
    public SessionHostProperties? Properties { get; set; }
}