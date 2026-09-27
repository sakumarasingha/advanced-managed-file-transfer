import { PublicClientApplication, type Configuration } from '@azure/msal-browser'

export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_MSAL_CLIENT_ID,
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_MSAL_TENANT_ID}`,
    // Computed at runtime rather than baked in at build time, so the same build works whether
    // it's served from localhost:5173 in dev or the production App Service origin - both need
    // to be registered as SPA redirect URIs on the app registration.
    redirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: 'sessionStorage',
  },
}

export const apiScopes = [import.meta.env.VITE_API_SCOPE]

export const msalInstance = new PublicClientApplication(msalConfig)
