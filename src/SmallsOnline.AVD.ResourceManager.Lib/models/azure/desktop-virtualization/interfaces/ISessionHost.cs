namespace SmallsOnline.AVD.ResourceManager.Lib.Models.Azure.DesktopVirtualization;

public interface ISessionHost
{
    string? Name { get; set; }
    string? ResourceId { get; set; }
    SessionHostProperties Properties { get; set; }
}