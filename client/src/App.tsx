import { RouterProvider } from 'react-router-dom'
import { Toaster } from '@/components/ui/sonner'
import { RequireAuth } from '@/app/RequireAuth'
import { NotProvisioned } from '@/app/NotProvisioned'
import { router } from '@/app/routes'
import { useCurrentUser, useNotProvisioned } from '@/hooks/useCurrentUser'

function AuthenticatedApp() {
  const { isLoading } = useCurrentUser()
  const notProvisioned = useNotProvisioned()

  if (isLoading) {
    return (
      <div className="flex h-screen items-center justify-center text-sm text-muted-foreground">Loading...</div>
    )
  }

  if (notProvisioned) {
    return <NotProvisioned />
  }

  return <RouterProvider router={router} />
}

function App() {
  return (
    <RequireAuth>
      <AuthenticatedApp />
      <Toaster />
    </RequireAuth>
  )
}

export default App
