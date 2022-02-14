# Assigning permissions

In order for the resource manager to work, the _user-assigned managed identity_ needs to have the necessary permissions to interact with the resources in Azure.

## Creating a custom role in Azure

To simplify the permissions needed, you'll need to create a custom role in your Azure subscription. I have already created a [custom role definition that you can utilize here](../azure/custom-roles/AVD-Resource-Manager.json).

### Permissions needed

> ⚠️ **Note** ⚠️
>  
> These permissions could change in the future to either allow more access or reduce access.

I designed the necessary permissions required to be as minimal as possible. This will keep the potential security risks very low on the managed identity. This table includes the permissions required and the reason for needing it.

| Permission Name | Reason |
| --- | --- |
| `Microsoft.Compute/virtualMachines/read` | To view basic details about the VM. |
| `Microsoft.Compute/virtualMachines/powerOff/action` | To initiate a power-off request to the VM. |
| `Microsoft.Compute/virtualMachines/deallocate/action` | To deallocate the VM's resources. |
| `Microsoft.Compute/virtualMachines/instanceView/read` | To view extra details about the VM (Primarily for getting the status of the VM). |
| `Microsoft.DesktopVirtualization/hostpools/read` | To view basic details about the hostpool. |
| `Microsoft.DesktopVirtualization/hostpools/sessionhosts/read` | To view details of the session hosts registered to the hostpool. |
| `Microsoft.DesktopVirtualization/hostpools/sessionhosts/usersessions/read` | To view active sessions on a session host. |

### Creating the role

#### Through a template

You can deploy the custom role to your subscription automatically by using the [ARM template](../azure/templates/create-custom-role.json) ([Bicep version](../azure/templates/create-custom-role.bicep)). This will create the role in your subscription with all the necessary permissions.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FSmalls1652%2FSmallsOnline.AVD.ResourceManager%2Fmain%2Fazure%2Fcreate-custom-role.json)

#### Manually

For guidance on creating the role in Azure, [follow this guide from Microsoft](https://docs.microsoft.com/en-us/azure/role-based-access-control/custom-roles-portal).

You can either assign the permissions by hand or [use the JSON template](../azure/custom-roles/AVD-Resource-Manager.json) as a starting point. The only thing you need to do is to make sure that the **Assignable scopes** is set to your subscription.

### Assigning the role

> ⚠️ **Note** ⚠️
>  
> If you created the role manually, it will be named whatever you named it.
>  
> If you created the role using the template, it will be called **`Azure Virtual Desktop Resource Manager (2022.01.00)`**.

Once the role has been created, assign the custom role to the _**user-assigned managed identity**_, that was created for the resource manager, on the **resource group** that houses the hostpool and it's VMs. For guidance on how to assign roles in Azure, [follow this guide from Microsoft](https://docs.microsoft.com/en-us/azure/role-based-access-control/role-assignments-portal).
