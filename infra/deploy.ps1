<#
.SYNOPSIS
  Creates/updates the free App Service from main.bicep, then builds and deploys the API + UI to it.

.EXAMPLE
  ./infra/deploy.ps1 -ResourceGroup rg-amft-ae-dev
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $ResourceGroup,
    [string] $Location = 'australiaeast'
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent

function Assert-Success([string] $what) {
    if ($LASTEXITCODE -ne 0) { throw "$what failed (exit code $LASTEXITCODE)." }
}

Write-Host "==> Provisioning infrastructure in '$ResourceGroup'"
az group create --name $ResourceGroup --location $Location --output none
Assert-Success 'Creating resource group'

$outputs = az deployment group create `
    --resource-group $ResourceGroup `
    --parameters "$PSScriptRoot/main.bicepparam" `
    --query properties.outputs --output json | ConvertFrom-Json
Assert-Success 'Bicep deployment'

$webApp = $outputs.webAppName.value
$url    = $outputs.webAppUrl.value

Write-Host "==> Publishing Mft.Api (also builds the React UI into wwwroot)"
$publishDir = Join-Path $repoRoot 'publish'
$zip        = Join-Path $repoRoot 'publish.zip'
Remove-Item $publishDir, $zip -Recurse -Force -ErrorAction SilentlyContinue
dotnet publish (Join-Path $repoRoot 'src/Mft.Api') -c Release -o $publishDir
Assert-Success 'dotnet publish'

# tar (bsdtar, built into Windows 10+) writes forward-slash zip entries; Compress-Archive in
# Windows PowerShell 5.1 writes backslashes, which Linux App Service extracts as flat files.
tar -a -cf $zip -C $publishDir .
Assert-Success 'Creating zip'

Write-Host "==> Deploying to $webApp"
az webapp deploy --resource-group $ResourceGroup --name $webApp --src-path $zip --type zip --clean true
Assert-Success 'Zip deploy'

Write-Host "==> Allowing sign-in from $url"
$clientId = (Get-Content (Join-Path $repoRoot 'src/Mft.Api/appsettings.json') -Raw | ConvertFrom-Json).AzureAd.ClientId
$app      = az ad app show --id $clientId --query '{id:id, uris:spa.redirectUris}' --output json | ConvertFrom-Json
Assert-Success 'Reading app registration'
$uris = @(@($app.uris) + $url | Select-Object -Unique)
$bodyFile = New-TemporaryFile
@{ spa = @{ redirectUris = $uris } } | ConvertTo-Json -Depth 4 | Set-Content $bodyFile -Encoding utf8
az rest --method PATCH --uri "https://graph.microsoft.com/v1.0/applications/$($app.id)" `
    --headers 'Content-Type=application/json' --body "@$bodyFile"
Assert-Success 'Updating redirect URIs'
Remove-Item $bodyFile

Write-Host ""
Write-Host "Deployed: $url"
Write-Host ""
Write-Host "One manual step remains - give the app's managed identity access to the database."
Write-Host "Connect to the database as an Entra admin and run:"
Write-Host "  CREATE USER [$webApp] FROM EXTERNAL PROVIDER;"
Write-Host "  ALTER ROLE db_datareader ADD MEMBER [$webApp];"
Write-Host "  ALTER ROLE db_datawriter ADD MEMBER [$webApp];"
Write-Host "  ALTER ROLE db_ddladmin   ADD MEMBER [$webApp];"
if ($outputs.deployIdentityClientId.value) {
    Write-Host ""
    Write-Host "For GitHub Actions deployments, add these repository secrets:"
    Write-Host "  AZURE_CLIENT_ID       = $($outputs.deployIdentityClientId.value)"
    Write-Host "  AZURE_TENANT_ID       = $($outputs.tenantId.value)"
    Write-Host "  AZURE_SUBSCRIPTION_ID = $($outputs.subscriptionId.value)"
}
