/*
Name: Deploy resources for Azure Virtual Desktop Resource Manager
Author: Tim Small
Website: https://smalls.online
GitHub Repo: https://github.com/Smalls1652/SmallsOnline.AVD.ResourceManager
Version: 2022.01.00

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
param databaseName string

@minLength(1)
@description('The name of what you want the Azure Functions app to be called.')
param functionAppName string

var managedIdentityName = '${functionAppName}-identity'
var functionAppSvcPlanName = '${replace(resourceGroup().name, '-', '')}-ASP-${take(uniqueString(resourceGroup().id), 4)}'
var storageAcctName = '${take(uniqueString(resourceGroup().id), 6)}${take(replace(resourceGroup().name, '-', ''),6)}stg'

// Create the Azure CosmosDB account.
resource cosmosDbResource 'Microsoft.DocumentDB/databaseAccounts@2021-10-15' = {
  location: location
  name: databaseName

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
    allowBlobPublicAccess: true
    supportsHttpsTrafficOnly: true

    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true
          keyType: 'Account'
        }

        file: {
          enabled: true
          keyType: 'Account'
        }
      }
    }
  }
}

// Create the blob services in the storage account.
resource storageAcctBlobServices 'Microsoft.Storage/storageAccounts/blobServices@2021-06-01' = {
  parent: storageAcctResource
  name: 'default'
  properties: {
    deleteRetentionPolicy: {
      enabled: false
    }
  }
}

// Create a blob container named 'azure-webjobs-hosts' in the storage account.
// Disable public access to the container and blobs in it.
resource storageAcctBlobWebJobsHostsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-06-01' = {
  parent: storageAcctBlobServices
  name: 'azure-webjobs-hosts'
  properties: {
    publicAccess: 'None'
    immutableStorageWithVersioning: {
      enabled: false
    }
  }
}

// Create a blob container named 'azure-webjobs-secrets' in the storage account.
// Disable public access to the container and blobs in it.
resource storageAcctBlobWebJobsSecretsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-06-01' = {
  parent: storageAcctBlobServices
  name: 'azure-webjobs-secrets'
  properties: {
    publicAccess: 'None'
    immutableStorageWithVersioning: {
      enabled: false
    }
  }
}

// Create a blob container named 'function-releases' in the storage account.
// Disable public access to the container and blobs in it.
resource storageAcctBlobFunctionReleasesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-06-01' = {
  parent: storageAcctBlobServices
  name: 'function-releases'
  properties: {
    publicAccess: 'None'
    immutableStorageWithVersioning: {
      enabled: false
    }
  }
}

// Create a blob container named 'scm-releases' in the storage account.
// Disable public access to the container and blobs in it.
resource storageAcctBlobScmReleasesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-06-01' = {
  parent: storageAcctBlobServices
  name: 'scm-releases'
  properties: {
    publicAccess: 'None'
    immutableStorageWithVersioning: {
      enabled: false
    }
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
    perSiteScaling: false
    elasticScaleEnabled: false
    maximumElasticWorkerCount: 1
    isSpot: false
    reserved: true
    isXenon: false
    hyperV: false
    targetWorkerCount: 0
    targetWorkerSizeId: 0
    zoneRedundant: false
  }
}

// Create the Functions app.
resource functionAppResource 'Microsoft.Web/sites@2021-02-01' = {
  name: functionAppName
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

    siteConfig: {
      numberOfWorkers: 1
      linuxFxVersion: 'DOTNET|6.0'
      functionAppScaleLimit: 200
      minimumElasticInstanceCount: 1
    }

    redundancyMode: 'None'

    serverFarmId: functionAppServicePlanResource.id
  }
}

// Set the Functions app web config.
resource functionAppConfigWeb 'Microsoft.Web/sites/config@2021-02-01' = {
  parent: functionAppResource
  name: 'web'

  properties: {
    numberOfWorkers: 1
    netFrameworkVersion: 'v4.0'
    linuxFxVersion: 'DOTNET|6.0'
    alwaysOn: false
    functionAppScaleLimit: 200
    functionsRuntimeScaleMonitoringEnabled: false
    minimumElasticInstanceCount: 1
  }
}

// Set the default Functions app AppSettings.
resource functionAppConfigAppSettings 'Microsoft.Web/sites/config@2021-02-01' = {
  parent: functionAppResource
  name: 'appsettings'
  
  properties: {
    AzureSubscriptionId: subscription().subscriptionId
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${storageAcctResource.name};AccountKey=${storageAcctResource.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
    CosmosDbConnectionString: 'AccountEndpoint=https://${cosmosDbResource.name}.documents.azure.com:443/;AccountKey=${cosmosDbResource.listKeys().primaryMasterKey};'
    CosmosDbDatabaseName: cosmosDbDatabase.name
    FUNCTIONS_EXTENSION_VERSION: '~4'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet-isolated'
    ManagedIdentityClientId: managedIdentityResource.properties.clientId
  }
}

// Set the Functions app hostname binding.
resource functionAppHostNameBinding 'Microsoft.Web/sites/hostNameBindings@2021-02-01' = {
  parent: functionAppResource
  name: '${functionAppName}.azurewebsites.net'
  properties: {
    siteName: functionAppName
  }
}

// Output the resource IDs of all of the created resources.
output functionAppSvcPlanResourceId string = functionAppServicePlanResource.id
output functionAppResourceId string = functionAppResource.id
output functionAppStorageAccountResourceId string = storageAcctResource.id
output cosmosDbResourceId string = cosmosDbResource.id
output managedIdentityResourceId string = managedIdentityResource.id
