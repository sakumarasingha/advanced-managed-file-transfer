using './main.bicep'

param appName = 'amft-emdflow-ae-dev'

param sqlServerFqdn = 'sqlserver-emd-ae-prd.database.windows.net'
param sqlDatabaseName = 'free-sql-db-9758318'

param appSettings = {
  AzureAd__TenantId: 'b23fed04-2cba-4d2f-942a-c712f4b20564'
  AzureAd__ClientId: '25b995a2-ac50-4bfe-ab73-ca985b07ce94'
  // No AzureAd__Audience: the app registration issues v2 tokens (aud = client ID GUID), and setting
  // it to api://<clientId> would reject them. Leaving it unset accepts both forms.
}

param githubRepo ='sakumarasingha/advanced-managed-file-transfer'
param githubBranch = 'main'
param githubOidcSubject = 'repo:sakumarasingha@33798933/advanced-managed-file-transfer@1375215702:ref:refs/heads/main'
