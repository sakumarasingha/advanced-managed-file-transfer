using './main.bicep'

param appName = 'amft-emdflow-ae-dev'

param sqlServerFqdn = 'sqlserver-emd-ae-prd.database.windows.net'
param sqlDatabaseName = 'free-sql-db-9758318'

param appSettings = {
  AzureAd__TenantId: 'b23fed04-2cba-4d2f-942a-c712f4b20564'
  AzureAd__ClientId: '25b995a2-ac50-4bfe-ab73-ca985b07ce94'
  AzureAd__Audience: 'api://25b995a2-ac50-4bfe-ab73-ca985b07ce94'
}

param githubRepo ='sakumarasingha/advanced-managed-file-transfer'
param githubBranch = 'main'
param githubOidcSubject = 'repo:sakumarasingha@33798933/advanced-managed-file-transfer@1375215702:ref:refs/heads/main'
