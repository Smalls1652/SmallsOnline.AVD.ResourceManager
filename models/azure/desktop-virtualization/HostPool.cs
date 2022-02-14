using System.Text.Json.Serialization;

namespace SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

public class HostPool : IHostPool
{
    public HostPool() {}

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("location")]
    public string Location { get; set; } = default!;

    [JsonPropertyName("properties")]
    public HostPoolProperties Properties { get; set; } = default!;
}