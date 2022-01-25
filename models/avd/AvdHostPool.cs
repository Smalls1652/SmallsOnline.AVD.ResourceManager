namespace SmallsOnline.AVD.ResourceManager.Models.AVD;

public class AvdHostPool : IAvdHostPool
{
    public AvdHostPool() {}

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("partitionKey")]
    public string? PartitionKey { get; set; }

    [JsonPropertyName("hostPoolName")]
    public string? HostPoolName { get; set; }

    [JsonPropertyName("hostPoolResourceId")]
    public string? HostPoolResourceId { get; set; }
}