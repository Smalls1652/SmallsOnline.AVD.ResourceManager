namespace SmallsOnline.AVD.ResourceManager.Models.AVD;

/// <summary>
/// Data for an Azure Virtual Desktop hostpool.
/// 
/// Note: This is the data that goes into the database.
/// </summary>
public class AvdHostPool : IAvdHostPool
{
    public AvdHostPool() {}

    /// <summary>
    /// A unique identifier for the item.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// The partition key to use in the database.
    /// </summary>
    [JsonPropertyName("partitionKey")]
    public string? PartitionKey { get; set; }

    /// <summary>
    /// The name of the Azure Virtual Desktop hostpool.
    /// </summary>
    [JsonPropertyName("hostPoolName")]
    public string? HostPoolName { get; set; }

    /// <summary>
    /// The resource ID of the Azure Virtual Desktop hostpool.
    /// </summary>
    [JsonPropertyName("hostPoolResourceId")]
    public string? HostPoolResourceId { get; set; }
}