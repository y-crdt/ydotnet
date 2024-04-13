import { BrowserRouter, useRoutes } from 'react-router-dom';
import './App.css';
import config from './config';
import { YjsContextProvider } from './context/yjsContext';
import Home from './pages/Home';
import Tldraw from './pages/Tldraw';

const Routes = () =>
    useRoutes([
        {
            path: '/',
            element: (
                <YjsContextProvider
                    baseUrl={`${config.WS_URL}/collaboration`}
                    roomName="app"
                >
                    <Home />
                </YjsContextProvider>
            ),
        },
        {
            path: '/tldraw',
            element: (
                <YjsContextProvider
                    baseUrl={`${config.WS_URL}/collaboration`}
                    roomName="tldraw"
                >
                    <Tldraw />
                </YjsContextProvider>
            ),
        },
    ]);

function App() {
    return (
        <BrowserRouter>
            <Routes />
        </BrowserRouter>
    );
}

export default App;
