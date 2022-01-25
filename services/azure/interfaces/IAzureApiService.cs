using Azure.ResourceManager.Compute;

using SmallsOnline.AVD.ResourceManager.Models.AVD;
using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public interface IAzureApiService
{
    VirtualMachine GetAzVM(string resourceId);
    List<SessionHost>? GetSessionHosts(AvdHostPool hostPool);
}