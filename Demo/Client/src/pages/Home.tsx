import { Col, Container, Row } from 'reactstrap';
import { Awareness } from '../components/Awareness';
import { Chat } from '../components/Chat';
import { Increment } from '../components/Increment';
import { YjsMonacoEditor } from '../components/YjsMonacoEditor';
import { YjsProseMirror } from '../components/YjsProseMirror';
import config from '../config';
import { YjsContextProvider } from '../context/yjsContext';

function Home() {
    return (
        <>
            <Container>
                <Row className="mt-5">
                    <Col>
                        <h2>Awareness</h2>

                        <Awareness />
                    </Col>
                </Row>
                <Row className="mt-5">
                    <Col>
                        <h2>Monaco Editor</h2>

                        <YjsMonacoEditor />
                    </Col>
                </Row>
                <Row className="mt-5">
                    <Col>
                        <h2>Prose Mirror</h2>

                        <YjsProseMirror />
                    </Col>
                </Row>
                <Row className="mt-5">
                    <Col>
                        <h2>Increment</h2>

                        <Increment />
                    </Col>
                </Row>
                <Row className="mt-5 pb-5">
                    <Col>
                        <Row>
                            <Col>
                                <h2>Chat</h2>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <h5>Chat</h5>

                                <Chat isReadonly={false} />
                            </Col>
                            <Col>
                                <h5>Notifications</h5>

                                <YjsContextProvider
                                    baseUrl={`${config.WS_URL}/collaboration`}
                                    roomName="notifications"
                                >
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

export default Home;
