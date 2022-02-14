using SmallsOnline.AVD.ResourceManager.Models.AVD;
using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public interface ICosmosDbService
{
    List<AvdHost>? GetAvdHosts();
    AvdHostPool GetAvdHostPool(string id);
    List<AvdHostPool> GetAvdHostPools();
    AvdHost? GetAvdHost(string id);
    void UpdateAvdHost(AvdHost hostItem);
    void AddHostPool(HostPool hostPool);
}