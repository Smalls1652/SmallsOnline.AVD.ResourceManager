# How to deploy to Azure

## Using the pre-made template

The simplest solution will be to utilize [the Azure ARM template that's included in this repository here](./deploy-avd-rscmgr.json) ([Bicep version](./deploy-avd-rscmgr.bicep)). You can click the button below to deploy the template directly from the Azure Portal.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FSmalls1652%2FSmallsOnline.AVD.ResourceManager%2Fmain%2Fazure%2Fdeploy-avd-rscmgr.json)

You will need to do the following:

- Specify an existing resource group or create a new resource group.
  - I **highly** suggest creating a new resource group.
- Specify the name you want the Azure CosmosDB account to be called.
- Specify the name you want the Azure Functions app to be called.

Here's an example:

![Example of template parameters in the Azure portal](../.github/repo-imgs/deploy-to-azure_template/deployment-parameters.png)
