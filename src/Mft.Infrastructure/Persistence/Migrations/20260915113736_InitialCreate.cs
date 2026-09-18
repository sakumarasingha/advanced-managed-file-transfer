using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mft.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlanTier = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntraObjectId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntraTenantId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InvitedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvitedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastLoginAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalSftpConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Host = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AuthMethod = table.Column<int>(type: "int", nullable: false),
                    PasswordEncrypted = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PrivateKeyEncrypted = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PassphraseEncrypted = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    HostKeyFingerprint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalSftpConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalSftpConnections_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NamingConventions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PathTemplate = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NamingConventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NamingConventions_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PgpKeyPairs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    KeyType = table.Column<int>(type: "int", nullable: false),
                    KeySizeBits = table.Column<int>(type: "int", nullable: true),
                    PgpMode = table.Column<int>(type: "int", nullable: false),
                    PublicKeyArmored = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrivateKeyEncrypted = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PassphraseEncrypted = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Fingerprint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PgpKeyPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PgpKeyPairs_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystemDefined = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SftpLocalUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    HomeContainer = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    HomeDirectory = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    AuthMethods = table.Column<int>(type: "int", nullable: false),
                    HasPasswordSet = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProvisioningStatus = table.Column<int>(type: "int", nullable: false),
                    ExternalLocalUserId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastProvisioningError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SftpLocalUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SftpLocalUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StorageAccountSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageAccountName = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    ResourceGroup = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SubscriptionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SftpEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastTestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastTestSucceeded = table.Column<bool>(type: "bit", nullable: true),
                    LastTestMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageAccountSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageAccountSettings_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleAssignments_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleAssignments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleAssignments_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SftpUserPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SftpLocalUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContainerName = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    PathPrefix = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Permissions = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SftpUserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SftpUserPermissions_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SftpUserPermissions_SftpLocalUsers_SftpLocalUserId",
                        column: x => x.SftpLocalUserId,
                        principalTable: "SftpLocalUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SftpUserSshKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SftpLocalUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicKeyOpenSsh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fingerprint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SftpUserSshKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SftpUserSshKeys_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SftpUserSshKeys_SftpLocalUsers_SftpLocalUserId",
                        column: x => x.SftpLocalUserId,
                        principalTable: "SftpLocalUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferEndpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Container = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: true),
                    FolderPath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    SftpLocalUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalSftpConnectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RemotePath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferEndpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferEndpoints_ExternalSftpConnections_ExternalSftpConnectionId",
                        column: x => x.ExternalSftpConnectionId,
                        principalTable: "ExternalSftpConnections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferEndpoints_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferEndpoints_SftpLocalUsers_SftpLocalUserId",
                        column: x => x.SftpLocalUserId,
                        principalTable: "SftpLocalUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransferRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SourceEndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetEndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileTypeResolutionMode = table.Column<int>(type: "int", nullable: false),
                    FileTypeSubfolder = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NamingConventionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArchiveEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ArchiveEndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ErrorEndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleType = table.Column<int>(type: "int", nullable: false),
                    PickupIntervalMinutes = table.Column<int>(type: "int", nullable: true),
                    CronExpression = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TimeZoneId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxFileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    MaxFileSizeLimitEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferRoutes_NamingConventions_NamingConventionId",
                        column: x => x.NamingConventionId,
                        principalTable: "NamingConventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRoutes_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRoutes_TransferEndpoints_ArchiveEndpointId",
                        column: x => x.ArchiveEndpointId,
                        principalTable: "TransferEndpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRoutes_TransferEndpoints_ErrorEndpointId",
                        column: x => x.ErrorEndpointId,
                        principalTable: "TransferEndpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRoutes_TransferEndpoints_SourceEndpointId",
                        column: x => x.SourceEndpointId,
                        principalTable: "TransferEndpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRoutes_TransferEndpoints_TargetEndpointId",
                        column: x => x.TargetEndpointId,
                        principalTable: "TransferEndpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransferHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransferRouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TriggeredBy = table.Column<int>(type: "int", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SourcePath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    DestinationPath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ArchivePath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ErrorFolderPath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RetryOfTransferHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    PgpApplied = table.Column<bool>(type: "bit", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferHistories_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferHistories_TransferHistories_RetryOfTransferHistoryId",
                        column: x => x.RetryOfTransferHistoryId,
                        principalTable: "TransferHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferHistories_TransferRoutes_TransferRouteId",
                        column: x => x.TransferRouteId,
                        principalTable: "TransferRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransformationSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransferRouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    PgpKeyPairId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransformationSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransformationSteps_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransformationSteps_PgpKeyPairs_PgpKeyPairId",
                        column: x => x.PgpKeyPairId,
                        principalTable: "PgpKeyPairs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransformationSteps_TransferRoutes_TransferRouteId",
                        column: x => x.TransferRouteId,
                        principalTable: "TransferRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_OrganizationId_Email",
                table: "AppUsers",
                columns: new[] { "OrganizationId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_OrganizationId_EntraTenantId_EntraObjectId",
                table: "AppUsers",
                columns: new[] { "OrganizationId", "EntraTenantId", "EntraObjectId" },
                unique: true,
                filter: "[EntraObjectId] IS NOT NULL AND [EntraTenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_OrganizationId_TimestampUtc",
                table: "AuditLogs",
                columns: new[] { "OrganizationId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalSftpConnections_OrganizationId",
                table: "ExternalSftpConnections",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_NamingConventions_OrganizationId",
                table: "NamingConventions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Slug",
                table: "Organizations",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Key",
                table: "Permissions",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PgpKeyPairs_OrganizationId",
                table: "PgpKeyPairs",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAssignments_AppUserId",
                table: "RoleAssignments",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAssignments_OrganizationId_AppUserId_RoleId",
                table: "RoleAssignments",
                columns: new[] { "OrganizationId", "AppUserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleAssignments_RoleId",
                table: "RoleAssignments",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_OrganizationId",
                table: "Roles",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SftpLocalUsers_OrganizationId_Username",
                table: "SftpLocalUsers",
                columns: new[] { "OrganizationId", "Username" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SftpUserPermissions_OrganizationId",
                table: "SftpUserPermissions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SftpUserPermissions_SftpLocalUserId",
                table: "SftpUserPermissions",
                column: "SftpLocalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SftpUserSshKeys_OrganizationId",
                table: "SftpUserSshKeys",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SftpUserSshKeys_SftpLocalUserId",
                table: "SftpUserSshKeys",
                column: "SftpLocalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageAccountSettings_OrganizationId",
                table: "StorageAccountSettings",
                column: "OrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferEndpoints_ExternalSftpConnectionId",
                table: "TransferEndpoints",
                column: "ExternalSftpConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferEndpoints_OrganizationId",
                table: "TransferEndpoints",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferEndpoints_SftpLocalUserId",
                table: "TransferEndpoints",
                column: "SftpLocalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferHistories_OrganizationId_TransferRouteId_StartedAtUtc",
                table: "TransferHistories",
                columns: new[] { "OrganizationId", "TransferRouteId", "StartedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TransferHistories_RetryOfTransferHistoryId",
                table: "TransferHistories",
                column: "RetryOfTransferHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferHistories_TransferRouteId",
                table: "TransferHistories",
                column: "TransferRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRoutes_ArchiveEndpointId",
                table: "TransferRoutes",
                column: "ArchiveEndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRoutes_ErrorEndpointId",
                table: "TransferRoutes",
                column: "ErrorEndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRoutes_NamingConventionId",
                table: "TransferRoutes",
                column: "NamingConventionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRoutes_OrganizationId",
                table: "TransferRoutes",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRoutes_SourceEndpointId",
                table: "TransferRoutes",
                column: "SourceEndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRoutes_TargetEndpointId",
                table: "TransferRoutes",
                column: "TargetEndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_TransformationSteps_OrganizationId",
                table: "TransformationSteps",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransformationSteps_PgpKeyPairId",
                table: "TransformationSteps",
                column: "PgpKeyPairId");

            migrationBuilder.CreateIndex(
                name: "IX_TransformationSteps_TransferRouteId_Order",
                table: "TransformationSteps",
                columns: new[] { "TransferRouteId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "RoleAssignments");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SftpUserPermissions");

            migrationBuilder.DropTable(
                name: "SftpUserSshKeys");

            migrationBuilder.DropTable(
                name: "StorageAccountSettings");

            migrationBuilder.DropTable(
                name: "TransferHistories");

            migrationBuilder.DropTable(
                name: "TransformationSteps");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "PgpKeyPairs");

            migrationBuilder.DropTable(
                name: "TransferRoutes");

            migrationBuilder.DropTable(
                name: "NamingConventions");

            migrationBuilder.DropTable(
                name: "TransferEndpoints");

            migrationBuilder.DropTable(
                name: "ExternalSftpConnections");

            migrationBuilder.DropTable(
                name: "SftpLocalUsers");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
