namespace Mft.Application.Common;

public static class PermissionKeys
{
    public const string OrganizationManage = "organization.manage";
    public const string UsersManage = "users.manage";
    public const string RolesManage = "roles.manage";
    public const string ServerSetupManage = "serversetup.manage";
    public const string SftpUsersManage = "sftpusers.manage";
    public const string NamingConventionsManage = "namingconventions.manage";
    public const string TransferRoutesManage = "transferroutes.manage";
    public const string ExternalConnectionsManage = "externalconnections.manage";
    public const string PgpKeysManage = "pgpkeys.manage";
    public const string TransferHistoryView = "transferhistory.view";
    public const string TransferHistoryRetry = "transferhistory.retry";

    public static readonly IReadOnlyList<(string Key, string Category, string Description)> Catalog =
    [
        (OrganizationManage, "Organization", "View and edit organization settings"),
        (UsersManage, "Administration", "Invite, activate/deactivate users and assign roles"),
        (RolesManage, "Administration", "Create, edit and delete roles and their permissions"),
        (ServerSetupManage, "Server Setup", "Configure the Azure Storage account and test the SFTP connection"),
        (SftpUsersManage, "SFTP Users", "Create SFTP client users, manage their keys and permissions"),
        (NamingConventionsManage, "Naming Conventions", "Configure container/folder naming conventions"),
        (TransferRoutesManage, "Transfer Routes", "Configure source/target endpoints, transformation steps, archiving, error handling and schedules"),
        (ExternalConnectionsManage, "Transfer Routes", "Manage connection profiles (credentials) for third-party SFTP servers"),
        (PgpKeysManage, "PGP Keys", "Generate and manage PGP key pairs"),
        (TransferHistoryView, "Transfer History", "View file transfer history"),
        (TransferHistoryRetry, "Transfer History", "Retry a failed file transfer"),
    ];
}
