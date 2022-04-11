using Azure.ResourceManager.Compute;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
    /// <summary>
    /// Get details about a <see cref="VirtualMachine" /> in Azure.
    /// </summary>
    /// <param name="resourceId">The resource ID of the virtual machine.</param>
    /// <returns></returns>
    public VirtualMachineResource GetAzVM(string resourceId)
    {
        VirtualMachineResource vmItem = armClient.GetVirtualMachineResource(
            id: new(resourceId)
        );

        return vmItem;
    }
}