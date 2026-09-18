import type { PermissionKey } from '@/lib/permissions'
import { useCurrentUser } from './useCurrentUser'

export function usePermission(permission: PermissionKey): boolean {
  const { data } = useCurrentUser()
  return data?.permissions.includes(permission) ?? false
}
