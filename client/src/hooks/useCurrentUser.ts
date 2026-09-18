import { useQuery } from '@tanstack/react-query'
import { ApiError, apiClient } from '@/lib/apiClient'
import type { PermissionKey } from '@/lib/permissions'

export interface CurrentUser {
  userId: string
  organizationId: string
  organizationName: string
  displayName: string
  email: string
  roles: string[]
  permissions: PermissionKey[]
}

export function useCurrentUser() {
  return useQuery<CurrentUser>({
    queryKey: ['me'],
    queryFn: () => apiClient.get<CurrentUser>('/me'),
    retry: (failureCount, error) => {
      if (error instanceof ApiError && error.status === 403) {
        return false
      }
      return failureCount < 1
    },
  })
}

export function useNotProvisioned() {
  const { error } = useCurrentUser()
  return error instanceof ApiError && error.status === 403
}
