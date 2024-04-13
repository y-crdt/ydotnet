import { Col, Container, Row } from 'reactstrap';
import YjsTldrawEditor from '../components/YjsTldrawEditor';
import { useYjs } from '../hooks/useYjs';

function Tldraw() {
    const { roomName } = useYjs();
    return (
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
    );
}

export default Tldraw;
