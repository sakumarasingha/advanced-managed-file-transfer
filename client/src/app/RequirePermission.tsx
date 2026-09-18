import type { ReactNode } from 'react'
import { usePermission } from '@/hooks/usePermission'
import type { PermissionKey } from '@/lib/permissions'

export function RequirePermission({ permission, children }: { permission: PermissionKey; children: ReactNode }) {
  const allowed = usePermission(permission)

  if (!allowed) {
    return (
      <div className="flex flex-col items-center justify-center gap-2 py-24 text-center">
        <p className="text-lg font-medium">You don't have access to this section</p>
        <p className="text-sm text-muted-foreground">Ask your organization admin for the required permission.</p>
      </div>
    )
  }

  return <>{children}</>
}
