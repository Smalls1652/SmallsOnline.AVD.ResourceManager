namespace SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

public class SessionHostProperties : ISessionHostProperties
{
    public SessionHostProperties() {}

    [JsonPropertyName("objectId")]
    public string? ObjectId { get; set; }

    [JsonPropertyName("resourceId")]
    public string? ResourceId { get; set; }

    [JsonPropertyName("sessions")]
    public int? Sessions { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("assignedUser")]
    public string? AssignedUser { get; set; }
}