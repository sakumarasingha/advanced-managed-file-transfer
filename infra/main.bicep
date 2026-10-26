targetScope = 'resourceGroup'

@description('Short name used to derive resource names, e.g. "amft-emdflow-ae-dev" gives app-amft-emdflow-ae-dev, asp-amft-emdflow-ae-dev and id-amft-emdflow-ae-dev-deploy.')
@minLength(3)
@maxLength(40)
param appName string

@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('Fully qualified name of the existing Azure SQL server, e.g. myserver.database.windows.net.')
param sqlServerFqdn string

@description('Name of the existing Azure SQL database the API connects to.')
param sqlDatabaseName string

@description('GitHub repository (owner/name) allowed to deploy through OIDC. Leave empty to skip creating the CI/CD identity.')
param githubRepo string = ''

@description('Branch whose GitHub Actions runs are allowed to deploy.')
param githubBranch string = 'main'

@description('OIDC subject GitHub puts in its tokens. Repos that use ID-based subjects need repo:<owner>@<ownerId>/<repo>@<repoId>:ref:refs/heads/<branch>.')
param githubOidcSubject string = 'repo:${githubRepo}:ref:refs/heads/${githubBranch}'

param tags object = {}

var webAppName = 'app-${appName}'
var planName = 'asp-${appName}'
var deployIdentityName = 'id-${appName}-deploy'
var createDeployIdentity = !empty(githubRepo)
var websiteContributorRoleId = 'de139f84-1756-47ae-9be6-808fbbe84772'

resource plan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: planName
  location: location
  tags: tags
  kind: 'linux'
  sku: {
    name: 'F1'
    tier: 'Free'
  }
  properties: {
    reserved: true
  }
}

// One App Service hosts both the API and the React UI (the UI is built into wwwroot at publish time).
resource webApp 'Microsoft.Web/sites@2024-04-01' = {
  name: webAppName
  location: location
  tags: tags
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      // Explicit entry point so a stray *.runtimeconfig.json in the package can't change what starts.
      appCommandLine: 'dotnet Mft.Api.dll'
      alwaysOn: false // not available on the Free tier
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      appSettings: [
        {
          name: 'ConnectionStrings__Default'
          value: 'Server=tcp:${sqlServerFqdn},1433;Initial Catalog=${sqlDatabaseName};Encrypt=True;TrustServerCertificate=False;Authentication=Active Directory Managed Identity;'
        }
        {
          name: 'Storage__Mode'
          value: 'Mock'
        }
        {
          // The zip we deploy is already published output; don't let Oryx try to build it.
          name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
          value: 'false'
        }
      ]
    }
  }
}

resource logs 'Microsoft.Web/sites/config@2024-04-01' = {
  parent: webApp
  name: 'logs'
  properties: {
    applicationLogs: {
      fileSystem: {
        level: 'Information'
      }
    }
    httpLogs: {
      fileSystem: {
        enabled: true
        retentionInDays: 3
        retentionInMb: 35
      }
    }
    detailedErrorMessages: {
      enabled: true
    }
    failedRequestsTracing: {
      enabled: false
    }
  }
}

// Identity GitHub Actions signs in as (OIDC, no secrets), allowed to deploy to this web app only.
resource deployIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = if (createDeployIdentity) {
  name: deployIdentityName
  location: location
  tags: tags
}

resource githubFederation 'Microsoft.ManagedIdentity/userAssignedIdentities/federatedIdentityCredentials@2023-01-31' = if (createDeployIdentity) {
  parent: deployIdentity
  name: 'github-${githubBranch}'
  properties: {
    issuer: 'https://token.actions.githubusercontent.com'
    subject: githubOidcSubject
    audiences: [
      'api://AzureADTokenExchange'
    ]
  }
}

resource deployRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (createDeployIdentity) {
  name: guid(webApp.id, deployIdentityName, websiteContributorRoleId)
  scope: webApp
  properties: {
    principalId: deployIdentity!.properties.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', websiteContributorRoleId)
  }
}

output webAppName string = webApp.name
output webAppHostName string = webApp.properties.defaultHostName
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
@description('Object ID of the web app identity - needs a contained user in the SQL database.')
output webAppPrincipalId string = webApp.identity.principalId
output tenantId string = tenant().tenantId
output subscriptionId string = subscription().subscriptionId
@description('Value for the AZURE_CLIENT_ID GitHub secret (empty when githubRepo is not set).')
output deployIdentityClientId string = createDeployIdentity ? deployIdentity!.properties.clientId : ''
