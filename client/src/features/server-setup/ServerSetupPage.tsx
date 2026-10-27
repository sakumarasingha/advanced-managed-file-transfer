import { zodResolver } from '@hookform/resolvers/zod'
import { CheckCircle2, Loader2, XCircle } from 'lucide-react'
import { useState } from 'react'
import { Controller, useForm } from 'react-hook-form'
import { toast } from 'sonner'
import { z } from 'zod'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { PageHeader } from '@/components/shared/PageHeader'
import { ApiError } from '@/lib/apiClient'
import { usePermission } from '@/hooks/usePermission'
import { PermissionKeys } from '@/lib/permissions'
import {
  useContainers,
  useProvisionStorageAccount,
  useStorageAccountSettings,
  useTestConnection,
  useUpdateStorageAccountSettings,
} from './api'

// Configure creates a real storage account, so every field is required (the server re-validates).
const formSchema = z.object({
  storageAccountName: z
    .string()
    .min(3, 'Azure storage account names are 3-24 characters')
    .max(24, 'Azure storage account names are 3-24 characters')
    .regex(/^[a-z0-9]*$/, 'Lowercase letters and numbers only'),
  resourceGroup: z.string().min(1, 'Resource group is required').max(90),
  subscriptionId: z.string().uuid('Subscription ID must be a GUID'),
  sftpEnabled: z.boolean(),
})

type FormValues = z.infer<typeof formSchema>

export function ServerSetupPage() {
  const canManage = usePermission(PermissionKeys.ServerSetupManage)
  const { data: settings, isLoading } = useStorageAccountSettings()
  const updateSettings = useUpdateStorageAccountSettings()
  const testConnection = useTestConnection()
  const provisionStorageAccount = useProvisionStorageAccount()
  const { data: containers, refetch: refetchContainers, isFetching: containersLoading } = useContainers(false)

  const { control, handleSubmit, reset, formState } = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    values: settings
      ? {
          storageAccountName: settings.storageAccountName ?? '',
          resourceGroup: settings.resourceGroup ?? '',
          subscriptionId: settings.subscriptionId ?? '',
          sftpEnabled: settings.sftpEnabled,
        }
      : undefined,
  })

  // Valid form values waiting for the user to confirm; non-null while the confirmation dialog is open.
  const [pendingConfig, setPendingConfig] = useState<FormValues | null>(null)

  const onSubmit = handleSubmit((values) => setPendingConfig(values))

  const onConfirmConfigure = async () => {
    if (!pendingConfig) {
      return
    }

    try {
      await updateSettings.mutateAsync({
        storageAccountName: pendingConfig.storageAccountName,
        resourceGroup: pendingConfig.resourceGroup,
        subscriptionId: pendingConfig.subscriptionId,
        sftpEnabled: pendingConfig.sftpEnabled,
      })
    } catch {
      toast.error('Failed to save server setup')
      setPendingConfig(null)
      return
    }

    try {
      const result = await provisionStorageAccount.mutateAsync()
      if (result.succeeded) {
        toast.success(result.message)
      } else {
        toast.error(result.message)
      }
    } catch (error) {
      const message = error instanceof ApiError && typeof (error.body as { message?: string })?.message === 'string'
        ? (error.body as { message: string }).message
        : 'Settings were saved, but the storage account could not be provisioned.'
      toast.error(message)
    } finally {
      setPendingConfig(null)
    }
  }

  const onTestConnection = async () => {
    const result = await testConnection.mutateAsync()
    if (result.succeeded) {
      toast.success(result.message)
    } else {
      toast.error(result.message)
    }
  }

  const configuring = updateSettings.isPending || provisionStorageAccount.isPending

  if (isLoading) {
    return <div className="text-sm text-muted-foreground">Loading...</div>
  }

  return (
    <div className="max-w-2xl space-y-6">
      <PageHeader
        title="Account & Server Setup"
        description="Configure the Azure Storage account (SFTP server) this organization connects clients to."
      />

      <Card>
        <CardHeader>
          <CardTitle>Storage Account</CardTitle>
          <CardDescription>
            Configure creates this storage account in your Azure subscription (or updates it if it already
            exists). The resource group must already exist. Test Connection and the SFTP user operations still
            talk to a mock provider.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={onSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="storageAccountName">Storage account name</Label>
              <Controller
                name="storageAccountName"
                control={control}
                render={({ field }) => (
                  <Input id="storageAccountName" placeholder="mftprodstorage" disabled={!canManage} {...field} />
                )}
              />
              {formState.errors.storageAccountName && (
                <p className="text-xs text-destructive">{formState.errors.storageAccountName.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="resourceGroup">Resource group</Label>
              <Controller
                name="resourceGroup"
                control={control}
                render={({ field }) => (
                  <Input id="resourceGroup" placeholder="rg-mft-prod" disabled={!canManage} {...field} />
                )}
              />
              {formState.errors.resourceGroup && (
                <p className="text-xs text-destructive">{formState.errors.resourceGroup.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="subscriptionId">Subscription ID</Label>
              <Controller
                name="subscriptionId"
                control={control}
                render={({ field }) => (
                  <Input id="subscriptionId" placeholder="00000000-0000-0000-0000-000000000000" disabled={!canManage} {...field} />
                )}
              />
              {formState.errors.subscriptionId && (
                <p className="text-xs text-destructive">{formState.errors.subscriptionId.message}</p>
              )}
            </div>

            <div className="flex items-center justify-between rounded-md border p-3">
              <div>
                <p className="text-sm font-medium">SFTP enabled</p>
                <p className="text-xs text-muted-foreground">Enable the SFTP endpoint on this storage account.</p>
              </div>
              <Controller
                name="sftpEnabled"
                control={control}
                render={({ field }) => (
                  <div role="radiogroup" aria-label="SFTP enabled" className="flex gap-1">
                    {[true, false].map((option) => (
                      <Button
                        key={String(option)}
                        type="button"
                        role="radio"
                        aria-checked={field.value === option}
                        size="sm"
                        variant={field.value === option ? 'default' : 'outline'}
                        disabled={!canManage}
                        onClick={() => field.onChange(option)}
                      >
                        {option ? 'True' : 'False'}
                      </Button>
                    ))}
                  </div>
                )}
              />
            </div>

            {canManage && (
              <div className="flex gap-2 pt-2">
                <Button type="submit" disabled={configuring}>
                  {configuring && <Loader2 className="mr-2 size-4 animate-spin" />}
                  Configure
                </Button>
                <Button type="button" variant="outline" onClick={() => reset()} disabled={configuring}>
                  Reset
                </Button>
              </div>
            )}
          </form>
        </CardContent>
      </Card>

      <Dialog open={pendingConfig !== null} onOpenChange={(open) => !open && !configuring && setPendingConfig(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Configure storage account?</DialogTitle>
            <DialogDescription>
              This creates the storage account in Azure if it doesn't exist yet, or updates its SFTP setting if
              it does.
            </DialogDescription>
          </DialogHeader>
          {pendingConfig && (
            <dl className="grid grid-cols-[auto_1fr] gap-x-4 gap-y-1 text-sm">
              <dt className="text-muted-foreground">Storage account</dt>
              <dd className="font-medium">{pendingConfig.storageAccountName}</dd>
              <dt className="text-muted-foreground">Resource group</dt>
              <dd className="font-medium">{pendingConfig.resourceGroup}</dd>
              <dt className="text-muted-foreground">Subscription</dt>
              <dd className="font-mono text-xs">{pendingConfig.subscriptionId}</dd>
              <dt className="text-muted-foreground">SFTP enabled</dt>
              <dd className="font-medium">{pendingConfig.sftpEnabled ? 'True' : 'False'}</dd>
            </dl>
          )}
          <p className="text-xs text-muted-foreground">
            This creates a real Azure resource and may incur charges - including an hourly fee for as long as
            SFTP is enabled.
          </p>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setPendingConfig(null)} disabled={configuring}>
              Cancel
            </Button>
            <Button type="button" onClick={onConfirmConfigure} disabled={configuring}>
              {configuring && <Loader2 className="mr-2 size-4 animate-spin" />}
              Configure
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {canManage && (
        <Card>
          <CardHeader>
            <CardTitle>Connection</CardTitle>
            <CardDescription>Verify the storage account / SFTP configuration is reachable.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex items-center gap-3">
              <Button type="button" variant="secondary" onClick={onTestConnection} disabled={testConnection.isPending}>
                {testConnection.isPending && <Loader2 className="mr-2 size-4 animate-spin" />}
                Test Connection
              </Button>

              {settings?.lastTestedAtUtc && (
                <Badge variant={settings.lastTestSucceeded ? 'default' : 'destructive'} className="gap-1">
                  {settings.lastTestSucceeded ? <CheckCircle2 className="size-3" /> : <XCircle className="size-3" />}
                  {settings.lastTestSucceeded ? 'Connected' : 'Failed'}
                </Badge>
              )}
            </div>
            {settings?.lastTestMessage && (
              <p className="text-xs text-muted-foreground">{settings.lastTestMessage}</p>
            )}

            <div className="space-y-2 pt-2">
              <div className="flex items-center justify-between">
                <Label>Containers</Label>
                <Button type="button" size="sm" variant="ghost" onClick={() => refetchContainers()} disabled={containersLoading}>
                  {containersLoading && <Loader2 className="mr-2 size-3 animate-spin" />}
                  List containers
                </Button>
              </div>
              {containers && containers.length > 0 && (
                <div className="flex flex-wrap gap-2">
                  {containers.map((c) => (
                    <Badge key={c} variant="outline">
                      {c}
                    </Badge>
                  ))}
                </div>
              )}
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  )
}
