import { Col, Container, Row } from 'reactstrap';
import { YjsMonacoEditor } from './components/YjsMonacoEditor';
import { YjsProseMirror } from './components/YjsProseMirror';
import { Increment } from './components/Increment';
import './App.css';
import { YjsContextProvider } from './context/yjsContext';
import { Chat } from './components/Chat';

function App() {
    return (
        <>
            <Container>
                <Row className='mt-4'>
                    <Col>
                        <h2>Monaco Editor</h2>

                        <YjsMonacoEditor />
                    </Col>
                </Row>
                <Row className='mt-4'>
                    <Col>
                        <h2>Prose Mirror</h2>

                        <YjsProseMirror />
                    </Col>
                </Row>
                <Row className='mt-4'>
                    <Col>
                        <h2>Increment</h2>

                        <Increment />
                    </Col>
                </Row>
                <Row className='mt-4'>
                    <Col>
                        <Row>
                            <Col><h2>Chat</h2></Col>
                        </Row>
                        <Row>
                            <Col>
                                <h3>Chat</h3>

                                <Chat isReadonly={false} />
                            </Col>
                            <Col>
                                <h3>Notifications</h3>

                                <YjsContextProvider baseUrl={'ws://localhost:5000/collaboration'} roomName='notifications'>
                                    <Chat isReadonly />
                                </YjsContextProvider>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
        </>
    );
}

export default App;
