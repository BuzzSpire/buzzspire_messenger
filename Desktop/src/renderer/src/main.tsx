import './assets/main.css'

import React from 'react'
import ReactDOM from 'react-dom/client'
import './assets/base.css'
import { RouterProvider } from 'react-router-dom'
import { router } from './Routers'

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
)
