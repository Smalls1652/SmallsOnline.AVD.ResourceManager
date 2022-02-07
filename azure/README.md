# How to deploy to Azure

## Overview

In order to utilize Azure Virtual Desktop Resource Manager, you'll need to have all of the resources created. These are the following resources needed to deploy:

- Azure CosmosDB account
  - _It's suggested that it be configured as **"Consumption (Serverless)"** to reduce costs._
- Azure Functions app
  - _It's suggested that it be configured as **"Consumption (Serverless)"** to reduce costs._
- Azure Storage Account
- User-assigned managed identity

![A visualized sample of the resources.](../.github/repo-imgs/sample-resource-visualizer.png)

## Deployment methods

### Using the pre-made template

The simplest solution will be to utilize [the Azure ARM template that's included in this repository here](./deploy-avd-rscmgr.json) ([Bicep version](./deploy-avd-rscmgr.bicep)). You can click the button below to deploy the template directly from the Azure Portal.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FSmalls1652%2FSmallsOnline.AVD.ResourceManager%2Fmain%2Fazure%2Fdeploy-avd-rscmgr.json)

You will need to do the following:

- Specify an existing resource group or create a new resource group.
  - I **highly** suggest creating a new resource group.
- Specify the name you want the Azure CosmosDB account to be called.
- Specify the name you want the Azure Functions app to be called.

Here's an example:

![Example of template parameters in the Azure portal.](../.github/repo-imgs/deploy-to-azure_template/deployment-parameters.png)
