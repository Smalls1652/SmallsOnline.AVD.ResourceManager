using SmallsOnline.AVD.ResourceManager.Models.Database;
using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public interface ICosmosDbService
{
    List<SessionHostDbEntry>? GetAvdHosts(string? hostPoolResourceId = null);
    HostPoolDbEntry GetAvdHostPool(string id);
    List<HostPoolDbEntry> GetAvdHostPools();
    SessionHostDbEntry? GetAvdHost(string id);
    void UpdateAvdHost(SessionHostDbEntry hostItem);
    void AddHostPool(HostPool hostPool);
    void RemoveHostPool(HostPoolDbEntry hostPoolItem);
    void RemoveAvdHost(SessionHostDbEntry hostItem);
}