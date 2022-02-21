namespace SmallsOnline.AVD.ResourceManager.Models.Database;

public interface IHostPoolDbEntry
{
    string? Id { get; set; }
    string? PartitionKey { get; set; }
    string? HostPoolResourceId { get; set; }
}