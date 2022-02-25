namespace SmallsOnline.AVD.ResourceManager.Lib.Models.Database;

/// <summary>
/// Data for an Azure Virtual Desktop hostpool.
/// 
/// Note: This is the data that goes into the database.
/// </summary>
public class HostPoolDbEntry : IHostPoolDbEntry
{
    public HostPoolDbEntry() {}

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
    /// The resource ID of the Azure Virtual Desktop hostpool.
    /// </summary>
    [JsonPropertyName("hostPoolResourceId")]
    public string? HostPoolResourceId { get; set; }
}