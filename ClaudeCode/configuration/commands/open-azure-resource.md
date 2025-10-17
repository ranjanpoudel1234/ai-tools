# Open Azure Resources in Microsoft Edge

This command helps you quickly open various Azure resources in Microsoft Edge browser.

## Supported Azure Resources

Ask the user which Azure resource they want to open. Support the following options:

1. **Azure Portal** - Main portal dashboard
2. **Resource Group** - Specific resource group (ask for name)
3. **Storage Account** - Storage account (ask for name)
4. **App Service** - Web app or function app (ask for name)
5. **SQL Database** - SQL database (ask for server and database name)
6. **Key Vault** - Key vault (ask for name)
7. **Virtual Machine** - VM (ask for name)
8. **Kubernetes Service (AKS)** - AKS cluster (ask for name)
9. **Container Registry** - ACR (ask for name)
10. **Custom URL** - Any Azure portal URL provided by user

## Behavior

1. Ask the user which Azure resource type they want to open
2. If needed, ask for resource name(s) and subscription ID (optional)
3. Construct the appropriate Azure portal URL
4. Open the URL in Microsoft Edge using the command: `start msedge "URL"`

## Azure Portal URL Patterns

- Portal Home: `https://portal.azure.com/#home`
- Resource Group: `https://portal.azure.com/#@/resource/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/overview`
- Storage Account: `https://portal.azure.com/#@/resource/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Storage/storageAccounts/{accountName}/overview`
- App Service: `https://portal.azure.com/#@/resource/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Web/sites/{appName}/appServices`
- Key Vault: `https://portal.azure.com/#@/resource/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.KeyVault/vaults/{vaultName}/overview`
- Search for resource: `https://portal.azure.com/#blade/HubsExtension/BrowseAll`

If subscription ID or resource group is not provided, use the simplified search URL: `https://portal.azure.com/#blade/HubsExtension/BrowseAll` and let the user search in the portal.