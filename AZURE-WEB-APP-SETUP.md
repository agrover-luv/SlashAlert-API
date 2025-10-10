# 🚀 Azure Web App Setup for Free Tier

## 🎯 Quick Setup Instructions

Since you created a new Azure Web App with free tier, here's the simplest way to set up deployment:

### Step 1: Get Publish Profile from Azure

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to **App Services** → **slash-alert-api**
3. Click **"Get publish profile"** (top menu)
4. Download the `.publishsettings` file
5. Open the file in a text editor and copy all content

### Step 2: Add GitHub Secret

1. Go to your GitHub repository: `https://github.com/agrover-luv/SlashAlert-API`
2. Go to **Settings** → **Secrets and variables** → **Actions**
3. Click **"New repository secret"**
4. Create a secret:
   - **Name**: `AZUREAPPSERVICE_PUBLISHPROFILE`
   - **Value**: Paste the entire contents of the `.publishsettings` file
   - Click **"Add secret"**

### Step 3: Configure App Settings in Azure Portal

Since we're using publish profile (not Azure CLI), you'll need to manually set the Cosmos DB settings:

1. In Azure Portal, go to **App Services** → **slash-alert-api**
2. Go to **Configuration** → **Application settings**
3. Click **"New application setting"** and add:

| Name | Value |
|------|-------|
| `COSMOS_DB_ENDPOINT` | `https://slashalert-cosmos.documents.azure.com:443/` |
| `COSMOS_DB_PRIMARY_KEY` | Your Cosmos DB primary key from Azure Portal |

4. Click **"Save"** to apply changes

### Step 4: Required GitHub Secrets Summary

You only need this one secret for deployment:

| Secret Name | Description | How to Get |
|-------------|-------------|------------|
| `AZUREAPPSERVICE_PUBLISHPROFILE` | Azure Web App publish profile | Download from Azure Portal → App Service → Get publish profile |

## 🔄 How This Works

1. **Simpler Authentication**: Uses publish profile instead of complex federated identity
2. **Perfect for Free Tier**: No need for App Registration or service principals
3. **Direct Deployment**: GitHub Actions deploys directly to your web app
4. **Manual Config**: App settings configured once in Azure Portal

## 🧪 Test the Deployment

1. **Commit and push** your code
2. **Go to GitHub Actions** and watch the workflow run
3. **Check deployment** - should work without authentication errors
4. **Verify app settings** in Azure Portal
5. **Test your API** at your Azure Web App URL

## 🎯 Why This Fixes Your Issue

- **No App Registration needed**: Publish profile handles authentication
- **No federated identity**: Simpler for free tier apps
- **Direct deployment**: Bypass Azure CLI authentication issues
- **Works immediately**: No waiting for Azure AD propagation

This approach is perfect for development and free tier apps! 🚀

## 📋 Troubleshooting

If deployment still fails:
1. **Verify** the publish profile secret is correctly pasted
2. **Check** that the app name in workflow matches your Azure app name
3. **Ensure** your Azure Web App is running
4. **Try** re-downloading the publish profile if it's old

Once this works, you can optionally set up more advanced authentication later if needed.