namespace SmallsOnline.AVD.ResourceManager.Lib.Models.Azure.Generic;

public interface IAzureResponseCollection<T>
{
    List<T>? Value { get; set; }
}