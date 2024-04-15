import { Col, Container, Row } from 'reactstrap';
import YjsTldrawEditor from '../components/YjsTldrawEditor';
import { YjsTldrawContextProvider } from '../context/yjsTldrawContext';
import { useYjs } from '../hooks/useYjs';
import { YjsContextProvider } from '../context/yjsContext';

function TldrawPage() {
    return (
        <YjsContextProvider roomName="draw">
            <Inner />
        </YjsContextProvider>
    );
}

function Inner() {
    const { roomName } = useYjs();

    return (
        <YjsTldrawContextProvider id={roomName}>
            <Container>
                <Row className="mt-5">
                    <Col>
                        <h3>Tldraw Editor</h3>
                        <div style={{ height: 400 }}>
                            <YjsTldrawEditor />
                        </div>
                    </Col>
                </Row>
            </Container>
        </YjsTldrawContextProvider>
    );
}

export default TldrawPage;
