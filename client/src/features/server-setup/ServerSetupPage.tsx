import { zodResolver } from '@hookform/resolvers/zod'
import { CheckCircle2, Loader2, XCircle } from 'lucide-react'
import { Controller, useForm } from 'react-hook-form'
import { toast } from 'sonner'
import { z } from 'zod'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Switch } from '@/components/ui/switch'
import { PageHeader } from '@/components/shared/PageHeader'
import { usePermission } from '@/hooks/usePermission'
import { PermissionKeys } from '@/lib/permissions'
import {
  useContainers,
  useStorageAccountSettings,
  useTestConnection,
  useUpdateStorageAccountSettings,
} from './api'

const formSchema = z.object({
  storageAccountName: z
    .string()
    .max(24, 'Azure storage account names are at most 24 characters')
    .regex(/^[a-z0-9]*$/, 'Lowercase letters and numbers only')
    .optional()
    .or(z.literal('')),
  resourceGroup: z.string().max(200).optional().or(z.literal('')),
  subscriptionId: z.string().max(100).optional().or(z.literal('')),
  sftpEnabled: z.boolean(),
})

type FormValues = z.infer<typeof formSchema>

export function ServerSetupPage() {
  const canManage = usePermission(PermissionKeys.ServerSetupManage)
  const { data: settings, isLoading } = useStorageAccountSettings()
  const updateSettings = useUpdateStorageAccountSettings()
  const testConnection = useTestConnection()
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

  const onSubmit = handleSubmit(async (values) => {
    try {
      await updateSettings.mutateAsync({
        storageAccountName: values.storageAccountName || null,
        resourceGroup: values.resourceGroup || null,
        subscriptionId: values.subscriptionId || null,
        sftpEnabled: values.sftpEnabled,
      })
      toast.success('Server setup saved')
    } catch {
      toast.error('Failed to save server setup')
    }
  })

  const onTestConnection = async () => {
    const result = await testConnection.mutateAsync()
    if (result.succeeded) {
      toast.success(result.message)
    } else {
      toast.error(result.message)
    }
  }

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
            No real Azure Storage account is connected yet - these fields are placeholders until real
            Azure integration lands. Test Connection currently talks to a mock provider.
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
                  <Switch checked={field.value} onCheckedChange={field.onChange} disabled={!canManage} />
                )}
              />
            </div>

            {canManage && (
              <div className="flex gap-2 pt-2">
                <Button type="submit" disabled={updateSettings.isPending}>
                  {updateSettings.isPending && <Loader2 className="mr-2 size-4 animate-spin" />}
                  Save
                </Button>
                <Button type="button" variant="outline" onClick={() => reset()} disabled={updateSettings.isPending}>
                  Reset
                </Button>
              </div>
            )}
          </form>
        </CardContent>
      </Card>

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
