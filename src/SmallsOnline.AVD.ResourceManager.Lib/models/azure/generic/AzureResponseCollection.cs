namespace SmallsOnline.AVD.ResourceManager.Lib.Models.Azure.Generic;

/// <summary>
/// A collection of data returned by the Azure API.
/// </summary>
/// <typeparam name="T">The type the underlying data is using.</typeparam>
public class AzureResponseCollection<T> : IAzureResponseCollection<T>
{
    public AzureResponseCollection() {}

    /// <summary>
    /// A list of data returned by the API.
    /// </summary>
    [JsonPropertyName("value")]
    public List<T>? Value { get; set; }
}