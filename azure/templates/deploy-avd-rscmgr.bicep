/*
Name: Deploy resources for Azure Virtual Desktop Resource Manager
Author: Tim Small
Website: https://smalls.online
GitHub Repo: https://github.com/Smalls1652/SmallsOnline.AVD.ResourceManager
Version: 2022.01.01

Description:

This deployment template will create the necessary resources for Azure Virtual Desktop Resource Manager.

It creates the following resources in your Azure subscription:
- Azure CosmosDB account
  - Also creates a SQL database and container for the required resources inside the CosmosDB account.
  - The CosmosDB account is configured for 'serverless'.
- Azure Functions app
  - Configures the Function App's app settings for connecting to the resources it needs.
- Azure App Service plan
  - Used for the Functions App.
  - Is set to the 'serverless' tier.
- Azure Storage Account
  - Also creates the necessary blob containers for the Functions app.
- User-assigned managed identity
  - Used for the Function app to interact with the necessary resources it needs to interact with.
*/

@minLength(1)
@description('The location the resources should live at.')
param location string = resourceGroup().location

@minLength(1)
@description('The name of what you want the Azure CosmosDB account to be called.')
param databaseName string = 'avd-rscmgr-db'

@minLength(1)
@description('The name of what you want the Azure Functions app to be called.')
param functionAppName string = 'avd-rscmgr-func'

@minLength(6)
param randomHash string = utcNow()

var packageUrl = 'https://github.com/Smalls1652/SmallsOnline.AVD.ResourceManager/releases/download/v2022.01.04/SmallsOnline-AVD-ResourceManager_v2022.01.04.zip'

var uniqueNameString = uniqueString(subscription().id, randomHash)

var managedIdentityName = '${functionAppName}-identity-${uniqueNameString}'
var functionAppSvcPlanName = 'avd-rscmgr-ASP-${take(uniqueNameString, 6)}'
var storageAcctName = '${take(uniqueNameString, 12)}stg'

// Create the Azure CosmosDB account.
resource cosmosDbResource 'Microsoft.DocumentDB/databaseAccounts@2021-10-15' = {
  location: location
  name: '${databaseName}-${uniqueNameString}'

  kind: 'GlobalDocumentDB'
  identity: {
    type: 'None'
  }

  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
}

// Create a SQL database in the created CosmosDB account.
// The database name will be 'avd-resource-manager'.
resource cosmosDbDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-10-15' = {
  parent: cosmosDbResource
  name: 'avd-resource-manager'
  location: location
  properties: {
    resource: {
      id: 'avd-resource-manager'
    }
  }
}

// Create a container in the SQL database called 'monitored-hosts'.
resource cosmosDbDatabaseContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-10-15' = {
  parent: cosmosDbDatabase
  name: 'monitored-hosts'
  location: location
  properties: {
    resource: {
      id: 'monitored-hosts'

      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true

        includedPaths: [
          {
            path: '/*'
          }
        ]

        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }

      partitionKey: {
        kind: 'Hash'
        paths: [
          '/partitionKey'
        ]
      }
    }
  }
}

// Create the storage account for the Azure Function app.
resource storageAcctResource 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: storageAcctName
  location: location

  sku: {
    name: 'Standard_LRS'
  }

  kind: 'Storage'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

// Create the user assigned managed identity.
resource managedIdentityResource 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: managedIdentityName
  location: location
}

// Create the App Service plan to use for the Functions app.
// It will be a 'serverless' plan type.
resource functionAppServicePlanResource 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: functionAppSvcPlanName
  location: location

  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
    capacity: 0
  }
  
  kind: 'functionapp'
  properties: {
    reserved: true
  }
}

// Create the Functions app.
resource functionAppResource 'Microsoft.Web/sites@2021-02-01' = {
  name: '${functionAppName}-${uniqueNameString}'
  location: location

  kind: 'functionapp,linux'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityResource.id}': {}
    }
  }
  properties: {
    enabled: true
    reserved: true

    siteConfig: {
      linuxFxVersion: 'DOTNET|6.0'
      use32BitWorkerProcess: true
      numberOfWorkers: 1

      cors: {
        allowedOrigins: [
          'https://portal.azure.com'
        ]
      }

      appSettings: [
        {
          name: 'AzureSubscriptionId'
          value: subscription().subscriptionId
        }
        {
          name: 'AzureWebJobsStorage' 
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAcctResource.name};AccountKey=${storageAcctResource.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
        }
        {
          name: 'CosmosDbConnectionString'
          value: 'AccountEndpoint=https://${cosmosDbResource.name}.documents.azure.com:443/;AccountKey=${cosmosDbResource.listKeys().primaryMasterKey};'
        }
        {
          name: 'CosmosDbDatabaseName'
          value: cosmosDbDatabase.name
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'ManagedIdentityClientId'
          value: managedIdentityResource.properties.clientId
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: packageUrl
        }
      ]
    }

    clientAffinityEnabled: false

    serverFarmId: functionAppServicePlanResource.id
  }
}

// Output the resource IDs of all of the created resources.
output functionAppSvcPlanResourceId string = functionAppServicePlanResource.id
output functionAppResourceId string = functionAppResource.id
output functionAppStorageAccountResourceId string = storageAcctResource.id
output cosmosDbResourceId string = cosmosDbResource.id
output managedIdentityResourceId string = managedIdentityResource.id
