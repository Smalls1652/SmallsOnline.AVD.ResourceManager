using Azure.ResourceManager.Compute;

using SmallsOnline.AVD.ResourceManager.Models.Database;
using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public interface IAzureApiService
{
    VirtualMachine GetAzVM(string resourceId);
    List<SessionHost>? GetSessionHosts(HostPoolDbEntry hostPool);
    List<HostPool>? GetHostPools();
}