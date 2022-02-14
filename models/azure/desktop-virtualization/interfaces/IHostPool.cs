namespace SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

public interface IHostPool
{
    string Name { get; set; }
    string Id { get; set; }
    string Location { get; set; }
    HostPoolProperties Properties { get; set; }
}