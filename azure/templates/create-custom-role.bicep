/*
Name: Create custom role for user-assigned managed identity
Author: Tim Small
Website: https://smalls.online
GitHub Repo: https://github.com/Smalls1652/SmallsOnline.AVD.ResourceManager
Version: 2022.01.00

Description:

This deployment template will create a custom role in your Azure subscription to assign to the managed identity.
*/
@minLength(0)
param uniqueHash string = utcNow()

targetScope = 'subscription'

var roleGuid = guid(uniqueHash, subscription().subscriptionId, subscription().tenantId)

resource customRoleItem 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' = {
  name: roleGuid
  properties: {
    roleName: 'Azure Virtual Desktop Resource Manager (2022.01.00)'
    description: 'Permissions for the Azure Virtual Desktop Resource Manager\'s managed identity to interact with the hostpools and VMs it monitors.'

    assignableScopes: [
      subscription().id
    ]

    permissions: [
      {
        actions: [
          'Microsoft.Compute/virtualMachines/read'
          'Microsoft.Compute/virtualMachines/powerOff/action'
          'Microsoft.Compute/virtualMachines/deallocate/action'
          'Microsoft.Compute/virtualMachines/instanceView/read'
          'Microsoft.DesktopVirtualization/hostpools/read'
          'Microsoft.DesktopVirtualization/hostpools/sessionhosts/read'
          'Microsoft.DesktopVirtualization/hostpools/sessionhosts/usersessions/read'
        ]
      }
    ]
  }
}
