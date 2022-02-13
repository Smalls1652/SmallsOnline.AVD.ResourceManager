namespace SmallsOnline.AVD.ResourceManager.Models.AVD;

public interface IAvdHostPool
{
    string? Id { get; set; }
    string? PartitionKey { get; set; }
    string? HostPoolResourceId { get; set; }
}