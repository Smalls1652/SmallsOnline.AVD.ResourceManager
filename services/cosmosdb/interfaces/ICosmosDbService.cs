using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public interface ICosmosDbService
{
    List<AvdHost>? GetAvdHosts();
    AvdHostPool GetAvdHostPool(string id);
    List<AvdHostPool> GetAvdHostPools();
    AvdHost? GetAvdHost(string id);
    void UpdateAvdHost(AvdHost hostItem);
}