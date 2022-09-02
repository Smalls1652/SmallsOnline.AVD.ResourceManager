namespace SmallsOnline.AVD.ResourceManager.Lib.Models.Azure.DesktopVirtualization;

public interface IHostPool
{
    string Name { get; set; }
    string Id { get; set; }
    string Location { get; set; }
    HostPoolProperties Properties { get; set; }
    Dictionary<string, string>? Tags { get; set; }
}