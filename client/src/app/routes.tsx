import { Navigate, createBrowserRouter } from 'react-router-dom'
import { PermissionKeys } from '@/lib/permissions'
import { ServerSetupPage } from '@/features/server-setup/ServerSetupPage'
import { TransferHistoryPage } from '@/features/transfer-history/TransferHistoryPage'
import { TransferRoutesPage } from '@/features/transfer-routes/TransferRoutesPage'
import { NamingConventionsPage } from '@/features/naming-conventions/NamingConventionsPage'
import { SftpUsersPage } from '@/features/sftp-users/SftpUsersPage'
import { PgpKeysPage } from '@/features/pgp-keys/PgpKeysPage'
import { UsersPage } from '@/features/users/UsersPage'
import { RolesPage } from '@/features/roles/RolesPage'
import { AppShell } from './AppShell'
import { RequirePermission } from './RequirePermission'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppShell />,
    children: [
      { index: true, element: <Navigate to="/setup" replace /> },
      {
        path: 'history',
        element: (
          <RequirePermission permission={PermissionKeys.TransferHistoryView}>
            <TransferHistoryPage />
          </RequirePermission>
        ),
      },
      {
        path: 'setup',
        element: (
          <RequirePermission permission={PermissionKeys.ServerSetupManage}>
            <ServerSetupPage />
          </RequirePermission>
        ),
      },
      {
        path: 'transfer-routes',
        element: (
          <RequirePermission permission={PermissionKeys.TransferRoutesManage}>
            <TransferRoutesPage />
          </RequirePermission>
        ),
      },
      {
        path: 'naming-conventions',
        element: (
          <RequirePermission permission={PermissionKeys.NamingConventionsManage}>
            <NamingConventionsPage />
          </RequirePermission>
        ),
      },
      {
        path: 'sftp-users',
        element: (
          <RequirePermission permission={PermissionKeys.SftpUsersManage}>
            <SftpUsersPage />
          </RequirePermission>
        ),
      },
      {
        path: 'pgp-keys',
        element: (
          <RequirePermission permission={PermissionKeys.PgpKeysManage}>
            <PgpKeysPage />
          </RequirePermission>
        ),
      },
      {
        path: 'users',
        element: (
          <RequirePermission permission={PermissionKeys.UsersManage}>
            <UsersPage />
          </RequirePermission>
        ),
      },
      {
        path: 'roles',
        element: (
          <RequirePermission permission={PermissionKeys.RolesManage}>
            <RolesPage />
          </RequirePermission>
        ),
      },
    ],
  },
])
