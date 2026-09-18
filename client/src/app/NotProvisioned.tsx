import { useMsal } from '@azure/msal-react'
import { Button } from '@/components/ui/button'

export function NotProvisioned() {
  const { instance } = useMsal()

  return (
    <div className="flex h-screen flex-col items-center justify-center gap-3 text-center">
      <h1 className="text-xl font-semibold">You're signed in, but not set up yet</h1>
      <p className="max-w-md text-sm text-muted-foreground">
        Your Microsoft Entra ID account isn't provisioned in this organization's Managed File Transfer
        instance. Ask your organization admin to invite you, then sign in again.
      </p>
      <Button variant="outline" onClick={() => instance.logoutRedirect()}>
        Sign out
      </Button>
    </div>
  )
}
