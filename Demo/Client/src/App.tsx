import { Col, Container, Row } from 'reactstrap';
import { YjsMonacoEditor } from './components/YjsMonacoEditor';
import { YjsProseMirror } from './components/YjsProseMirror';
import { Increment } from './components/Increment';
import './App.css';
import { YjsContextProvider } from './context/yjsContext';
import { Chat } from './components/Chat';
import { Awareness } from './components/Awareness';

function App() {
    return (
        <>
            <Container>
                <Row className='mt-5'>
                    <Col>
                        <h2>Awareness</h2>

                        <Awareness />
                    </Col>
                </Row>
                <Row className='mt-5'>
                    <Col>
                        <h2>Monaco Editor</h2>

                        <YjsMonacoEditor />
                    </Col>
                </Row>
                <Row className='mt-5'>
                    <Col>
                        <h2>Prose Mirror</h2>

                        <YjsProseMirror />
                    </Col>
                </Row>
                <Row className='mt-5'>
                    <Col>
                        <h2>Increment</h2>

                        <Increment />
                    </Col>
                </Row>
                <Row className='mt-5 pb-5'>
                    <Col>
                        <Row>
                            <Col><h2>Chat</h2></Col>
                        </Row>
                        <Row>
                            <Col>
                                <h5>Chat</h5>

                                <Chat isReadonly={false} />
                            </Col>
                            <Col>
                                <h5>Notifications</h5>

                                <YjsContextProvider baseUrl={'ws://localhost:5000/collaboration2'} roomName='notifications'>
                                    <Chat isReadonly />
                                    
                                    <Awareness />
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
