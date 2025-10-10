# 🔧 Azure Federated Identity Configuration Fix

## 🚨 Current Issue

Your deployment is failing with this error:
```
AADSTS700213: No matching federated identity record found for presented assertion subject 'repo:agrover-luv/SlashAlert-API:environment:Production'
```

This means the Azure federated identity credential isn't properly configured for your GitHub environment.

## 🔐 Solution: Configure Federated Identity in Azure

### Step 1: Access Azure Portal

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to **Azure Active Directory** (or **Microsoft Entra ID**)
3. Go to **App registrations**
4. Find your app registration (using the client ID from your GitHub secrets)

### Step 2: Add Federated Credential

1. Click on your app registration
2. Go to **Certificates & secrets** in the left menu
3. Click on **Federated credentials** tab
4. Click **Add credential**

### Step 3: Configure Credential Settings

Use these **EXACT** values:

| Field | Value |
|-------|--------|
| **Federated credential scenario** | GitHub Actions deploying Azure resources |
| **Organization** | `agrover-luv` |
| **Repository** | `SlashAlert-API` |
| **Entity type** | `Environment` |
| **Environment name** | `Production` |
| **Name** | `SlashAlert-API-Production` |
| **Description** | `GitHub Actions deployment for SlashAlert API Production environment` |

### Step 4: Verify Configuration

The credential will automatically generate these values:
- **Issuer**: `https://token.actions.githubusercontent.com`
- **Subject identifier**: `repo:agrover-luv/SlashAlert-API:environment:Production`
- **Audiences**: `api://AzureADTokenExchange`

Make sure these match what's shown in your error message.

## 🔍 Alternative Solutions

### Option 1: Use Service Principal Authentication (Fallback)

If federated identity continues to cause issues, you can temporarily use a service principal:

1. Create a service principal secret in Azure
2. Add these GitHub secrets:
   - `AZURE_CLIENT_SECRET` (the service principal secret)
3. Update the workflow login step:

```yaml
- name: Login to Azure
  uses: azure/login@v2
  with:
    creds: ${{ secrets.AZURE_CREDENTIALS }}
```

Where `AZURE_CREDENTIALS` is a JSON secret containing:
```json
{
  "clientId": "your-client-id",
  "clientSecret": "your-client-secret", 
  "subscriptionId": "your-subscription-id",
  "tenantId": "your-tenant-id"
}
```

### Option 2: Remove Environment Requirement

If you don't need environment protection, you can modify the workflow:

```yaml
deploy:
  runs-on: windows-latest
  needs: build
  # Remove the environment section entirely
  permissions:
    id-token: write
    contents: read
```

Then create a federated credential with:
- **Entity type**: `Branch`
- **GitHub branch name**: `main`

## 🧪 Testing the Fix

After configuring the federated identity:

1. **Commit and push** your changes
2. **Check the workflow run** in GitHub Actions
3. **Look for the debug output** to verify the subject matches
4. **Verify successful Azure login**

## 🔧 Debugging Tips

The workflow now includes debug output that will show:
- Repository name
- Current ref
- Expected subject identifier

Compare this with what's configured in Azure.

## 📋 Checklist

- [ ] Azure app registration exists
- [ ] Federated credential added with correct values
- [ ] Organization = `agrover-luv`
- [ ] Repository = `SlashAlert-API`
- [ ] Entity type = `Environment`
- [ ] Environment name = `Production`
- [ ] Subject matches: `repo:agrover-luv/SlashAlert-API:environment:Production`
- [ ] Workflow has correct permissions (id-token: write)
- [ ] GitHub secrets are properly configured

## 🚨 Common Mistakes to Avoid

1. **Wrong entity type**: Make sure it's "Environment", not "Branch"
2. **Case sensitivity**: Environment name must be exactly "Production"
3. **Missing permissions**: Workflow needs `id-token: write` permission
4. **Wrong repository name**: Must match exactly with GitHub repository
5. **Multiple credentials**: Don't create duplicate federated credentials

## 🔄 If Still Failing

If the issue persists:

1. **Double-check** all values in Azure federated credential
2. **Wait 5-10 minutes** for Azure changes to propagate
3. **Try re-running** the workflow
4. **Check Azure Active Directory logs** for more detailed error information
5. **Consider** temporarily using service principal authentication as fallback

## 📞 Need Help?

If you continue to experience issues:
1. Check the Azure Portal for any error messages
2. Verify the app registration has the correct API permissions
3. Ensure the subscription and tenant IDs are correct
4. Try creating a new federated credential with a different name

This should resolve your Azure authentication issue! 🎯