namespace SmallsOnline.AVD.ResourceManager.Models.AVD;

public interface IAvdHost
{
    string? Id { get; set; }
    string? PartitionKey { get; set; }
    string? HostPoolResourceId { get; set; }
    string? HostPoolName { get; set; }
    string? VmResourceId { get; set; }
    AvdHostStatus? HostStatus { get; set; }
}