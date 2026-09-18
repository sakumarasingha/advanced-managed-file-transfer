import { useIsAuthenticated, useMsal } from '@azure/msal-react'
import { type ReactNode, useEffect } from 'react'

export function RequireAuth({ children }: { children: ReactNode }) {
  const isAuthenticated = useIsAuthenticated()
  const { instance, inProgress } = useMsal()

  useEffect(() => {
    if (!isAuthenticated && inProgress === 'none') {
      instance.loginRedirect().catch(console.error)
    }
  }, [isAuthenticated, inProgress, instance])

  if (!isAuthenticated) {
    return (
      <div className="flex h-screen items-center justify-center text-sm text-muted-foreground">
        Redirecting to sign in...
      </div>
    )
  }

  return <>{children}</>
}
