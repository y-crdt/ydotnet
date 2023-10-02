import { Col, Container, Row } from 'reactstrap';
import { YjsMonacoEditor } from './components/YjsMonacoEditor';
import { YjsProseMirror } from './components/YjsProseMirror';
import { Increment } from './components/Increment';
import './App.css';

function App() {
    return (
        <>
            <Container>
                <Row className='mt-4'>
                    <Col>
                        <h1>Monaco Editor</h1>

                        <YjsMonacoEditor />
                    </Col>
                </Row>
                <Row className='mt-4'>
                    <Col>
                        <h1>Prose Mirror</h1>

                        <YjsProseMirror />
                    </Col>
                </Row>
                <Row className='mt-4'>
                    <Col>
                        <h1>Increment</h1>

                        <Increment />
                    </Col>
                </Row>
            </Container>
        </>
    );
}

export default App;
