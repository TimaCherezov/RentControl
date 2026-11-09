import { createRoot } from 'react-dom/client'
import './index.css'
import { RouterProvider } from 'react-router-dom'
import { Providers } from './app/providers.tsx'
import { router } from './app/router.tsx'

createRoot(document.getElementById('root')!).render(
  <Providers>
    <RouterProvider router={router}></RouterProvider>
  </Providers>
)
