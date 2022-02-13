namespace SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

/// <summary>
/// Property data of an Azure Virtual Desktop session host.
/// 
/// <seealso href="https://docs.microsoft.com/en-us/rest/api/desktopvirtualization/session-hosts/get#sessionhost" />
/// </summary>
public class SessionHostProperties : ISessionHostProperties
{
    public SessionHostProperties() {}

    /// <summary>
    /// The unique object ID of the Azure Virtual Desktop session host.
    /// </summary>
    [JsonPropertyName("objectId")]
    public string ObjectId { get; set; } = default!;

    /// <summary>
    /// The resource ID of the session host.
    /// </summary>
    [JsonPropertyName("resourceId")]
    public string ResourceId { get; set; } = default!;

    /// <summary>
    /// The count of sessions currently active on the session host.
    /// </summary>
    [JsonPropertyName("sessions")]
    public int? Sessions { get; set; }

    /// <summary>
    /// The current status of the session host.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Whether the session host allows new sessions or not.
    /// </summary>
    [JsonPropertyName("allowNewSession")]
    public bool AllowNewSession { get; set; }

    /// <summary>
    /// The user assigned to the session host, if applicable.
    /// </summary>
    [JsonPropertyName("assignedUser")]
    public string? AssignedUser { get; set; }
}