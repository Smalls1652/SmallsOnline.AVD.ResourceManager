namespace SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

public interface ISessionHostProperties
{
    string? ObjectId { get; set; }
    string? ResourceId { get; set; }
    int? Sessions { get; set; }
    string? Status { get; set; }
    bool AllowNewSession { get; set; }
    string? AssignedUser { get; set; }
}