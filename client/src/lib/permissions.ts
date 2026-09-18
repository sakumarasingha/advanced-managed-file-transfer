// Mirrors Mft.Application.Common.PermissionKeys on the backend - keep in sync.
export const PermissionKeys = {
  OrganizationManage: 'organization.manage',
  UsersManage: 'users.manage',
  RolesManage: 'roles.manage',
  ServerSetupManage: 'serversetup.manage',
  SftpUsersManage: 'sftpusers.manage',
  NamingConventionsManage: 'namingconventions.manage',
  TransferRoutesManage: 'transferroutes.manage',
  ExternalConnectionsManage: 'externalconnections.manage',
  PgpKeysManage: 'pgpkeys.manage',
  TransferHistoryView: 'transferhistory.view',
  TransferHistoryRetry: 'transferhistory.retry',
} as const

export type PermissionKey = (typeof PermissionKeys)[keyof typeof PermissionKeys]
