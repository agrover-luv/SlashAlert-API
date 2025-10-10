# 🔧 Azure Federated Identity Setup Instructions

## 🚨 Root Cause

The error `AADSTS700213: No matching federated identity record found` occurs because Azure doesn't have a federated identity credential configured that matches your GitHub workflow.

**Current GitHub assertion subject**: `repo:agrover-luv/SlashAlert-API:ref:refs/heads/main`

## 🎯 Immediate Fix: Configure Azure Federated Identity

### Step 1: Access Azure Portal

1. Go to [Azure Portal](https://portal.azure.com)
2. Search for **"App registrations"** 
3. Find your app registration (look for the one with Client ID that matches your GitHub secret)

### Step 2: Add Federated Credential

1. Click on your App registration
2. In the left menu, click **"Certificates & secrets"**
3. Click the **"Federated credentials"** tab
4. Click **"Add credential"**

### Step 3: Configure Credential (Use These EXACT Values)

| Field | Value |
|-------|--------|
| **Federated credential scenario** | GitHub Actions deploying Azure resources |
| **Organization** | `agrover-luv` |
| **Repository** | `SlashAlert-API` |
| **Entity type** | `Branch` |
| **GitHub branch name** | `main` |
| **Name** | `SlashAlert-API-main-branch` |

This will create a credential with:
- **Subject**: `repo:agrover-luv/SlashAlert-API:ref:refs/heads/main`
- **Issuer**: `https://token.actions.githubusercontent.com`
- **Audiences**: `api://AzureADTokenExchange`

### Step 4: Verify App Registration Permissions

Make sure your app registration has these API permissions:
- **Azure Service Management** → `user_impersonation`
- **Microsoft Graph** → `User.Read` (optional)

## 🔄 Alternative: Service Principal Authentication

If federated identity continues to cause issues, you can use service principal authentication:

### Option A: Create Azure Credentials Secret

1. In Azure Portal, go to your App registration
2. Go to **Certificates & secrets** → **Client secrets**
3. Click **New client secret**
4. Copy the secret value

5. In GitHub, add a new secret called `AZURE_CREDENTIALS` with this JSON:
```json
{
  "clientId": "your-client-id-here",
  "clientSecret": "your-client-secret-here",
  "subscriptionId": "your-subscription-id-here",
  "tenantId": "your-tenant-id-here"
}
```

### Option B: Use Individual Secrets (Current Approach)

The workflow is already configured to fall back to individual secrets if needed.

## 🧪 Test the Configuration

1. **Commit and push** any changes
2. **Go to GitHub Actions** and check the workflow run
3. **Look for the debug output** showing the expected subject
4. **Verify** it matches what you configured in Azure

## 📋 Troubleshooting Checklist

- [ ] App registration exists in Azure
- [ ] Federated credential added with correct values
- [ ] Organization = `agrover-luv` (exact match)
- [ ] Repository = `SlashAlert-API` (exact match)
- [ ] Entity type = `Branch` (not Environment)
- [ ] Branch name = `main`
- [ ] App registration has proper permissions
- [ ] Wait 5-10 minutes for Azure changes to propagate

## 🚨 If Still Failing

If you still get authentication errors:

1. **Double-check** all Azure configuration values
2. **Try deleting and recreating** the federated credential
3. **Verify** the client ID in GitHub secrets matches Azure
4. **Consider** using service principal authentication temporarily
5. **Check** Azure Active Directory audit logs for detailed errors

This should resolve your authentication issue! 🎯