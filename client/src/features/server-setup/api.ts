import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/lib/apiClient'
import type {
  StorageAccountSettings,
  TestConnectionResponse,
  UpdateStorageAccountSettingsRequest,
} from './types'

const queryKey = ['serversetup', 'storage-account']

export function useStorageAccountSettings() {
  return useQuery<StorageAccountSettings>({
    queryKey,
    queryFn: () => apiClient.get<StorageAccountSettings>('/serversetup/storage-account'),
  })
}

export function useUpdateStorageAccountSettings() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (data: UpdateStorageAccountSettingsRequest) =>
      apiClient.put<StorageAccountSettings>('/serversetup/storage-account', data),
    onSuccess: (data) => {
      queryClient.setQueryData(queryKey, data)
    },
  })
}

export function useTestConnection() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: () => apiClient.post<TestConnectionResponse>('/serversetup/test-connection'),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey })
    },
  })
}

export function useContainers(enabled: boolean) {
  return useQuery<string[]>({
    queryKey: ['serversetup', 'containers'],
    queryFn: () => apiClient.get<string[]>('/serversetup/containers'),
    enabled,
  })
}
