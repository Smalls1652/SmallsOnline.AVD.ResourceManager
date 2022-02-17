using Azure.ResourceManager.Compute;

using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

namespace SmallsOnline.AVD.ResourceManager.Models.Database;

/// <summary>
/// Data for an Azure Virtual Desktop session host.
/// 
/// Note: This is the data that goes into the database.
/// </summary>
public class SessionHostDbEntry : ISessionHostDbEntry
{
    [JsonConstructor()]
    public SessionHostDbEntry() { }
    public SessionHostDbEntry(VirtualMachineData virtualMachineData, HostPoolDbEntry avdHostPool, SessionHost avdSessionHostData, SessionHostDbEntry? previousHostData)
    {
        Id = avdSessionHostData.Properties?.ObjectId;
        PartitionKey = "avd-host-items";
        HostPoolResourceId = avdHostPool.HostPoolResourceId;
        VmResourceId = virtualMachineData.Id;

        if (previousHostData is not null)
        {
            NoSessionsCount = previousHostData.NoSessionsCount;
        }
        else
        {
            NoSessionsCount = 0;
        }
    }

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

    /// <summary>
    /// The resource ID for the virtual machine.
    /// </summary>
    [JsonPropertyName("vmResourceId")]
    public string? VmResourceId { get; set; }

    /// <summary>
    /// The amount of times the host has been identified as not having any active sessions.
    /// </summary>
    [JsonPropertyName("noSessionsCount")]
    public int? NoSessionsCount { get; set; }

    /// <summary>
    /// Increment the <see cref="NoSessionsCount">NoSessionsCount</see> property.
    /// </summary>
    public void IncrementNoSessionsCount()
    {
        NoSessionsCount++;
    }

    /// <summary>
    /// Reset the <see cref="NoSessionsCount">NoSessionsCount</see> property to 0.
    /// </summary>
    public void ResetNoSessionsCount()
    {
        NoSessionsCount = 0;
    }

    /// <summary>
    /// Evaluate if the <see cref="NoSessionsCount">NoSessionsCount</see> property is greater than or equal to the threshold count.
    /// </summary>
    /// <param name="threshold">The max amount the <see cref="NoSessionsCount">NoSessionsCount</see> property can go.</param>
    /// <returns></returns>
    public bool ShouldShutdown(int threshold)
    {
        return NoSessionsCount >= threshold;
    }
}