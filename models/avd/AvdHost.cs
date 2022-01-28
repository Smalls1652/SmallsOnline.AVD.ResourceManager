using Azure.ResourceManager.Compute;

using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

namespace SmallsOnline.AVD.ResourceManager.Models.AVD;

public class AvdHost : IAvdHost
{
    [JsonConstructor()]
    public AvdHost() { }
    public AvdHost(VirtualMachineData virtualMachineData, AvdHostPool avdHostPool, SessionHost avdSessionHostData, AvdHost? previousHostData)
    {
        Id = avdSessionHostData.Properties?.ObjectId;
        PartitionKey = "avd-host-items";
        HostPoolResourceId = avdHostPool.HostPoolResourceId;
        HostPoolName = avdHostPool.HostPoolName;
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

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("partitionKey")]
    public string? PartitionKey { get; set; }

    [JsonPropertyName("hostPoolResourceId")]
    public string? HostPoolResourceId { get; set; }

    [JsonPropertyName("hostPoolName")]
    public string? HostPoolName { get; set; }

    [JsonPropertyName("vmResourceId")]
    public string? VmResourceId { get; set; }

    [JsonPropertyName("noSessionsCount")]
    public int? NoSessionsCount { get; set; }

    public void IncrementNoSessionsCount()
    {
        NoSessionsCount++;
    }

    public void ResetNoSessionsCount()
    {
        NoSessionsCount = 0;
    }

    public bool ShouldShutdown(int threshold)
    {
        return NoSessionsCount >= threshold;
    }
}