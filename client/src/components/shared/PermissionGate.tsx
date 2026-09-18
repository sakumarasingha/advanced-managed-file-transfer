import type { ReactNode } from 'react'
import { usePermission } from '@/hooks/usePermission'
import type { PermissionKey } from '@/lib/permissions'

export function PermissionGate({ permission, children }: { permission: PermissionKey; children: ReactNode }) {
  const allowed = usePermission(permission)
  if (!allowed) {
    return null
  }
  return <>{children}</>
}
