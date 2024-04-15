import { Col, Container, Row } from 'reactstrap';
import YjsTldrawEditor from '../components/YjsTldrawEditor';
import { YjsTldrawContextProvider } from '../context/yjsTldrawContext';
import { useYjs } from '../hooks/useYjs';

function Tldraw() {
    const { roomName } = useYjs();
    return (
        <YjsTldrawContextProvider id={roomName}>
            <Container>
                <Row className="mt-5">
                    <Col>
                        <h2>Tldraw Editor</h2>
                        <div style={{ height: 400 }}>
                            <YjsTldrawEditor id={roomName} />
                        </div>
                    </Col>
                </Row>
            </Container>
        </YjsTldrawContextProvider>
    );
}

export default Tldraw;
