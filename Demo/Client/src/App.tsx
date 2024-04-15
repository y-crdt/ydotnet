import { BrowserRouter, NavLink, redirect, useRoutes } from 'react-router-dom';
import TldrawPage from './pages/Tldraw';
import { Col, Container, Nav, NavItem, Navbar, Row } from 'reactstrap';
import ChatPage from './pages/Chat';
import MonacoPage from './pages/Monaco';
import ProseMirrorPage from './pages/ProseMirror';
import IncrementPage from './pages/Increment';
import './App.css';

const Routes = () =>
    useRoutes([
        {
            path: '/chat',
            element: <ChatPage />,
        },
        {
            path: '/monaco',
            element: <MonacoPage />,
        },
        {
            path: '/prosemirror',
            element: <ProseMirrorPage />,
        },
        {
            path: '/increment',
            element: <IncrementPage />,
        },
        {
            path: '/tldraw',
            element: <TldrawPage />,
        },
        {
            path: '/',
            action: () => redirect('/monaco'),
        },
    ]);

function App() {
    return (
        <BrowserRouter>
            <Navbar color="dark" expand dark fixed='top'>
                <Nav navbar>
                    <NavItem>
                        <NavLink className="nav-link" to="/monaco">
                            Monaco
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink className="nav-link" to="/prosemirror">
                            Prosemirror
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink className="nav-link" to="/increment">
                            Increment
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink className="nav-link" to="/chat">
                            Chat
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink className="nav-link" to="/tldraw">
                            Draw
                        </NavLink>
                    </NavItem>
                </Nav>
            </Navbar>

            <Container style={{ paddingTop: '80px' }}>
                <Row>
                    <Col>
                        <h1 className='mb-4'>YDotNet Demo Page</h1>

                        <Routes />
                    </Col>
                </Row>
            </Container>
        </BrowserRouter>
    );
}

export default App;
