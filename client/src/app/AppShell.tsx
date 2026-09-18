import { useMsal } from '@azure/msal-react'
import {
  ArrowLeftRight,
  History,
  Key,
  ServerCog,
  Shield,
  Tags,
  Users,
  Users2,
} from 'lucide-react'
import { NavLink, Outlet } from 'react-router-dom'
import { cn } from '@/lib/utils'
import { useCurrentUser } from '@/hooks/useCurrentUser'
import type { PermissionKey } from '@/lib/permissions'
import { PermissionKeys } from '@/lib/permissions'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { Avatar, AvatarFallback } from '@/components/ui/avatar'

interface NavItem {
  to: string
  label: string
  icon: React.ComponentType<{ className?: string }>
  permission: PermissionKey
}

const navItems: NavItem[] = [
  { to: '/history', label: 'File Transfer History', icon: History, permission: PermissionKeys.TransferHistoryView },
  { to: '/setup', label: 'Account & Server Setup', icon: ServerCog, permission: PermissionKeys.ServerSetupManage },
  { to: '/transfer-routes', label: 'Transfer Routes', icon: ArrowLeftRight, permission: PermissionKeys.TransferRoutesManage },
  { to: '/naming-conventions', label: 'Naming Conventions', icon: Tags, permission: PermissionKeys.NamingConventionsManage },
  { to: '/sftp-users', label: 'SFTP Users & Permissions', icon: Users2, permission: PermissionKeys.SftpUsersManage },
  { to: '/pgp-keys', label: 'PGP Keys', icon: Key, permission: PermissionKeys.PgpKeysManage },
  { to: '/users', label: 'Users', icon: Users, permission: PermissionKeys.UsersManage },
  { to: '/roles', label: 'Roles & Permissions', icon: Shield, permission: PermissionKeys.RolesManage },
]

function initials(name: string) {
  return name
    .split(' ')
    .map((p) => p[0])
    .join('')
    .slice(0, 2)
    .toUpperCase()
}

export function AppShell() {
  const { instance } = useMsal()
  const { data: me } = useCurrentUser()

  return (
    <div className="flex h-screen">
      <aside className="flex w-64 shrink-0 flex-col border-r bg-sidebar text-sidebar-foreground">
        <div className="flex h-14 items-center gap-2 border-b border-sidebar-border px-4">
          <div className="flex size-7 items-center justify-center rounded-md bg-primary text-primary-foreground text-xs font-bold">
            MFT
          </div>
          <span className="font-semibold">Managed File Transfer</span>
        </div>

        <nav className="flex-1 space-y-1 overflow-y-auto p-2">
          {navItems.map((item) => {
            const allowed = me?.permissions.includes(item.permission) ?? true
            if (!allowed) return null

            const Icon = item.icon
            return (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) =>
                  cn(
                    'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
                    isActive
                      ? 'bg-sidebar-accent text-sidebar-accent-foreground'
                      : 'text-sidebar-foreground/80 hover:bg-sidebar-accent hover:text-sidebar-accent-foreground',
                  )
                }
              >
                <Icon className="size-4" />
                {item.label}
              </NavLink>
            )
          })}
        </nav>
      </aside>

      <div className="flex min-w-0 flex-1 flex-col">
        <header className="flex h-14 shrink-0 items-center justify-between border-b px-6">
          <span className="text-sm text-muted-foreground">{me?.organizationName}</span>

          <DropdownMenu>
            <DropdownMenuTrigger className="flex items-center gap-2 rounded-md px-2 py-1 text-sm hover:bg-accent">
              <Avatar className="size-7">
                <AvatarFallback className="text-xs">{me ? initials(me.displayName || me.email) : '?'}</AvatarFallback>
              </Avatar>
              <span>{me?.displayName || me?.email}</span>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuLabel>{me?.email}</DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => instance.logoutRedirect()}>Sign out</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </header>

        <main className="flex-1 overflow-y-auto p-6">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
