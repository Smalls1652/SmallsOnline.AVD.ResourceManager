using System.Text.Json.Serialization;

namespace SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

public class HostPoolProperties : IHostPoolProperties
{
    public HostPoolProperties() {}

    [JsonPropertyName("objectId")]
    public string ObjectId { get; set; } = default!;

    [JsonPropertyName("hostPoolType")]
    public string HostPoolType { get; set; } = default!;
}