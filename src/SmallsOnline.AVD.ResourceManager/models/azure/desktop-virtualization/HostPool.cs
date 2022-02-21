namespace SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

/// <summary>
/// Details, retrieved from the Azure API, about a hostpool.
/// </summary>
public class HostPool : IHostPool
{
    public HostPool() {}

    /// <summary>
    /// The name of the hostpool.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// The resource id of the hostpool.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    /// <summary>
    /// The Azure datacenter region the hostpool is located at.
    /// </summary>
    [JsonPropertyName("location")]
    public string Location { get; set; } = default!;

    /// <summary>
    /// Additional properties of the hostpool.
    /// </summary>
    [JsonPropertyName("properties")]
    public HostPoolProperties Properties { get; set; } = default!;
}