import { Col, Row } from 'reactstrap';
import { Awareness } from '../components/Awareness';
import { Chat } from '../components/Chat';
import { YjsContextProvider } from '../context/yjsContext';

function ChatPage() {
    return (
        <YjsContextProvider roomName="app">
            <Row>
                <Col>
                    <h3>Chat</h3>

                    <Chat isReadonly={false} />
                </Col>
                <Col>
                    <h3>Notifications</h3>

                    <YjsContextProvider roomName="notifications">
                        <Chat isReadonly />

                        <Awareness />
                    </YjsContextProvider>
                </Col>
            </Row>
        </YjsContextProvider>
    );
}

export default ChatPage;
