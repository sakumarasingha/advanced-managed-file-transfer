# Advanced Managed File Transfer (MFT)

A SaaS Managed File Transfer application: Azure Storage SFTP administration, per-organization
RBAC via Microsoft Entra ID, SFTP client/user management, PGP encryption, and configurable
transfer routes (source/destination, archiving, error handling, scheduling, file size limits,
naming conventions).

This is Milestone 1: full solution scaffold + a fully working **Account & Server Setup** section.
See `.claude/plans` (or ask the assistant) for the full milestone roadmap.

## Stack

- **Backend**: ASP.NET Core Web API on .NET 10 (C#), EF Core, SQL Server
- **Frontend**: React + TypeScript + Vite, shadcn/ui + Tailwind CSS v4
- **Auth**: Microsoft Entra ID (Microsoft.Identity.Web + MSAL React)

## Prerequisites

- .NET 10 SDK
- Node.js 20+ and npm
- A SQL Server-compatible database: Docker Desktop (local SQL Server via `docker compose up -d`),
  LocalDB, or a real Azure SQL Database - see "Database connection" below.

## Running locally

1. **Point at a database.** `appsettings.Development.json` ships a LocalDB fallback connection
   string (safe to commit - no real secret). To use something else (Docker SQL Server, or a real
   Azure SQL Database), set it via user-secrets instead of editing that file, so credentials never
   land in a tracked file:

   ```bash
   dotnet user-secrets set "ConnectionStrings:Default" "<your connection string>" --project src/Mft.Api
   ```

   For Azure SQL with Azure AD auth (`Authentication=Active Directory Default`), make sure you're
   logged in via `az login` first - it'll pick up your Azure CLI credential locally and switch to
   Managed Identity automatically when deployed to Azure.

2. **Apply migrations and seed data** (also happens automatically on every `dotnet run`, but you
   can do it standalone):

   ```bash
   dotnet tool restore
   dotnet dotnet-ef database update --project src/Mft.Infrastructure --startup-project src/Mft.Api
   ```

3. **Run the API**:

   ```bash
   dotnet run --project src/Mft.Api
   ```

   API listens on `https://localhost:7157` (see `src/Mft.Api/Properties/launchSettings.json`).
   Swagger UI is available at `/swagger` in development.

4. **Run the frontend:**

   ```bash
   cd client
   npm install
   cp .env.local.example .env.local   # fill in real Entra ID app registration values when you have them
   npm run dev
   ```

   Opens on `http://localhost:5173`, with `/api/*` proxied to the backend.

## Auth setup

`appsettings.json`/`client/.env.local` ship with either real or placeholder Entra ID values - if
they're still placeholder GUIDs, the app boots and shows the MSAL login screen but sign-in fails
with an `AADSTS700038` error until you wire up a real app registration:

1. Register an app in Microsoft Entra ID (one App Registration works for both the API and SPA).
   It needs: a SPA platform redirect URI for `http://localhost:5173` (and your deployed origin),
   and an exposed API scope (Expose an API -> set an Application ID URI -> add a delegated scope,
   e.g. `access_as_user`) so the SPA can request a token for its own API.
2. Update `AzureAd:TenantId` / `AzureAd:ClientId` / `AzureAd:Audience` in
   `src/Mft.Api/appsettings.json` (`Audience` = the Application ID URI, e.g. `api://<clientId>`).
3. Update `VITE_MSAL_CLIENT_ID` / `VITE_MSAL_TENANT_ID` / `VITE_API_SCOPE` in `client/.env.local`.
4. The very first sign-in claims a bootstrap "Org Admin" invite seeded for
   `DataSeeder.BootstrapAdminEmail` (`sakumarasingha@gmail.com` by default) in the default
   organization - update that constant if you want a different first admin.

## Solution layout

```
src/
  Mft.Domain/          entities, enums
  Mft.Application/     interfaces (IStorageSftpProvider, IPgpService, current-user/org context), DTOs
  Mft.Infrastructure/  EF Core DbContext + migrations, mock Azure Storage SFTP provider, DI
  Mft.Api/             ASP.NET Core host, controllers, auth, authorization policies
client/                React + Vite + TS app
```

## Storage integration

Azure Storage SFTP is implemented behind `IStorageSftpProvider`. Only a `MockStorageSftpProvider`
exists today (`Storage:Mode=Mock` in config) - a real `AzureStorageSftpProvider` backed by
`Azure.ResourceManager.Storage` / `Azure.Storage.Blobs` is a later milestone.
