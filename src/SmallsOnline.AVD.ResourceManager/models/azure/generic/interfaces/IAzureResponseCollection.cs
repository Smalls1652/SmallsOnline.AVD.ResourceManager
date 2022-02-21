namespace SmallsOnline.AVD.ResourceManager.Models.Azure.Generic;

public interface IAzureResponseCollection<T>
{
    List<T>? Value { get; set; }
}