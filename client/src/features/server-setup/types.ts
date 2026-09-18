export interface StorageAccountSettings {
  id: string
  storageAccountName: string | null
  resourceGroup: string | null
  subscriptionId: string | null
  sftpEnabled: boolean
  lastTestedAtUtc: string | null
  lastTestSucceeded: boolean | null
  lastTestMessage: string | null
}

export interface UpdateStorageAccountSettingsRequest {
  storageAccountName: string | null
  resourceGroup: string | null
  subscriptionId: string | null
  sftpEnabled: boolean
}

export interface TestConnectionResponse {
  succeeded: boolean
  message: string
}
