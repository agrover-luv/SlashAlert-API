# 🔐 GitHub Secrets Setup Guide

This guide explains how to securely configure your GitHub repository to use secrets instead of hardcoded credentials.

## 🚨 Security Issue Resolved

**Problem**: Cosmos DB credentials were stored directly in the repository
**Solution**: Use GitHub Secrets for production and environment variables

## 📋 Required GitHub Secrets

You need to add these secrets to your GitHub repository:

### 1. COSMOS_DB_ENDPOINT
- **Value**: `https://slashalert-cosmos.documents.azure.com:443/`
- **Description**: Your Azure Cosmos DB endpoint URL

### 2. COSMOS_DB_PRIMARY_KEY
- **Value**: `[YOUR_COSMOS_DB_PRIMARY_KEY_HERE]`
- **Description**: Your Azure Cosmos DB primary access key (get from Azure Portal)

### 3. AZURE_RESOURCE_GROUP (Optional)
- **Value**: `slash-alert-api_group` (or your actual resource group name)
- **Description**: Azure Resource Group containing your web app
- **Note**: If not provided, workflow will use default name "slash-alert-api_group"

## 🔧 How to Add GitHub Secrets

### Step 1: Go to Repository Settings
1. Navigate to your GitHub repository: `https://github.com/agrover-luv/SlashAlert-API`
2. Click on **Settings** tab
3. In the left sidebar, click **Secrets and variables** → **Actions**

### Step 2: Add Repository Secrets
1. Click **New repository secret**
2. Add the first secret:
   - **Name**: `COSMOS_DB_ENDPOINT`
   - **Value**: `https://slashalert-cosmos.documents.azure.com:443/`
   - Click **Add secret**

3. Add the second secret:
   - **Name**: `COSMOS_DB_PRIMARY_KEY`
   - **Value**: `[YOUR_COSMOS_DB_PRIMARY_KEY_FROM_AZURE_PORTAL]`
   - Click **Add secret**

## 🏗️ How It Works

### Development Environment
- Uses `appsettings.Local.json` with actual credentials (local only, gitignored)
- **Safe**: This file is only used locally and not deployed

### Production Environment
- Uses environment variables set by GitHub Secrets
- **Secure**: Credentials are encrypted and not visible in code

### Code Changes Made

#### 1. **appsettings.json** (Production)
```json
{
  "CosmosDb": {
    "EndpointUri": "",
    "PrimaryKey": "",
    // ... other settings
  }
}
```
- ✅ No secrets stored in production config
- ✅ Safe to commit to repository

#### 2. **Program.cs** (Smart Configuration)
- ✅ Uses environment variables in Production
- ✅ Uses configuration files in Development
- ✅ Clear error messages if secrets are missing

#### 3. **GitHub Workflow** (Secure Deployment)
- ✅ Sets environment variables from GitHub Secrets during deployment
- ✅ Uses Azure CLI to configure app settings
- ✅ Deploys to Azure with secure configuration
- ✅ Includes environment configuration for proper deployment tracking

## 🔄 Deployment Flow

1. **Code Push** → GitHub repository
2. **GitHub Actions** → Reads secrets securely
3. **Azure Deployment** → Sets environment variables
4. **Application** → Uses environment variables in production

## ✅ Security Benefits

- **No secrets in code**: Repository is safe to share
- **Encrypted storage**: GitHub encrypts secrets at rest
- **Audit trail**: GitHub logs secret usage
- **Access control**: Only authorized users can view/edit secrets
- **Environment separation**: Different secrets for different environments

## 🧪 Testing

### Local Development
```bash
# Uses appsettings.Local.json (create this file locally)
dotnet run
```

### Production Simulation
```bash
# Set environment variables locally
export COSMOS_DB_ENDPOINT="https://slashalert-cosmos.documents.azure.com:443/"
export COSMOS_DB_PRIMARY_KEY="your-key-here"

# Run in production mode
dotnet run --environment Production
```

## 🚨 Important Notes

1. **Never commit secrets**: Always use environment variables or secret management
2. **Rotate keys regularly**: Update both GitHub secrets and Azure Cosmos DB keys
3. **Minimum permissions**: Use least privilege access for service accounts
4. **Monitor access**: Regularly review who has access to secrets

## 📊 Configuration Priority

1. **Environment Variables** (Production)
2. **appsettings.Local.json** (Development - gitignored)
3. **appsettings.{Environment}.json** (Development - placeholder values)
4. **appsettings.json** (Fallback - no secrets)

This ensures maximum security while maintaining development convenience! 🔒