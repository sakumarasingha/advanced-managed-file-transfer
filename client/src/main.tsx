import { MsalProvider } from '@azure/msal-react'
import { QueryClientProvider } from '@tanstack/react-query'
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.tsx'
import './index.css'
import { msalInstance } from './lib/msalConfig'
import { queryClient } from './lib/queryClient'

async function bootstrap() {
  await msalInstance.initialize()

  const response = await msalInstance.handleRedirectPromise()
  if (response?.account) {
    msalInstance.setActiveAccount(response.account)
  } else {
    const accounts = msalInstance.getAllAccounts()
    if (accounts.length > 0) {
      msalInstance.setActiveAccount(accounts[0])
    }
  }

  createRoot(document.getElementById('root')!).render(
    <StrictMode>
      <MsalProvider instance={msalInstance}>
        <QueryClientProvider client={queryClient}>
          <App />
        </QueryClientProvider>
      </MsalProvider>
    </StrictMode>,
  )
}

bootstrap()
