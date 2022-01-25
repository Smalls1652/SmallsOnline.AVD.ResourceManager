namespace SmallsOnline.AVD.ResourceManager.Models.Azure.Generic;

public class AzureResponseCollection<T> : IAzureResponseCollection<T>
{
    public AzureResponseCollection() {}

    [JsonPropertyName("value")]
    public List<T>? Value { get; set; }
}