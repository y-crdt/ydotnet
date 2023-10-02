import * as React from 'react'
import * as ReactDOM from 'react-dom/client'
import { YjsContextProvider } from './context/yjsContext'
import App from './App';
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css'

ReactDOM.createRoot(document.getElementById('root')!).render(    
  <YjsContextProvider baseUrl={'ws://localhost:5000/collaboration'}>
    <App />
  </YjsContextProvider>,
)
