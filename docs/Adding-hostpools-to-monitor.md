# Adding hostpools to monitor

Currently, the only way to register hostpools is by adding an entry to the CosmosDB container manually. In the future, this may be automated by querying the resources that the _user-assigned managed identity_ has read-access to.

## Getting the resource ID of the hostpool

1. In the Azure portal, navigate to the hostpool resource.
2. When the pane for the hostpool is loaded, click the **Properties** link on the left-hand side of the pane.
3. Copy the text in the textbox labled **Resource ID**.
    - You can click the copy button on the far-right side of the textbox to quickly copy it.

## Adding an entry to the database

1. In the Azure portal, navigate to the _Azure CosmosDB_ resource that was created for this deployment.
2. When the pane for the hostpool is loaded, click the **Data Explorer** link on the left-hand side of the pane.
3. Once the **Data Explorer** has loaded, expand **avd-resource-manager**, then expand **monitored-hosts**, and then click **Items**.
4. At the top of the page, click the **New Item** button.
5. A new document should show up to the right-hand side of the window.
6. Copy the sample, [found below](#json-sample), and replace the contents of the document in the Azure portal with it.
7. Change the value of the `hostPoolResourceId` property with the **Resource ID** of the hostpool.
8. Click the **Save** button at the top of the window.

Whenever the Functions app executes next, it will start evaluating that hostpool.

### JSON Sample

```json
{
    "partitionKey": "avd-hostpool-items",
    "hostPoolResourceId": ""
}
```
