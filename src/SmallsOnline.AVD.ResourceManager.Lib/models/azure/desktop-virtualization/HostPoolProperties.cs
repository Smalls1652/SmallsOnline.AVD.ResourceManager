namespace SmallsOnline.AVD.ResourceManager.Lib.Models.Azure.DesktopVirtualization;

/// <summary>
/// Additional properties of the hostpool.
/// </summary>
public class HostPoolProperties : IHostPoolProperties
{
    public HostPoolProperties() {}

    /// <summary>
    /// The object id of the hostpool.
    /// </summary>
    [JsonPropertyName("objectId")]
    public string ObjectId { get; set; } = default!;

    /// <summary>
    /// The type of hostpool it is.
    /// </summary>
    [JsonPropertyName("hostPoolType")]
    public string HostPoolType { get; set; } = default!;
}