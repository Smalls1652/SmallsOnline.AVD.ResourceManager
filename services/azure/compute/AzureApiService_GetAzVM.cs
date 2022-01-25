using Azure.ResourceManager;
using Azure.ResourceManager.Compute;

using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
    public VirtualMachine GetAzVM(string resourceId)
    {
        VirtualMachine vmItem = armClient.GetVirtualMachine(
            id: new(resourceId)
        );

        return vmItem;
    }
}