using './main.bicep'

param appName = 'amft-emdflow-ae-dev'

param sqlServerFqdn = 'sqlserver-emd-ae-prd.database.windows.net'
param sqlDatabaseName = 'free-sql-db-9758318'

param githubRepo = 'sakumarasingha/advanced-managed-file-transfer'
param githubBranch = 'main'
param githubOidcSubject = 'repo:sakumarasingha@33798933/advanced-managed-file-transfer@1375215702:ref:refs/heads/main'
