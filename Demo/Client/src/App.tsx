import * as React from 'react';
import { Col, Container, Row } from 'reactstrap';
import { YjsMonacoEditor } from './components/YjsMonacoEditor'
import { YjsProseMirror } from './components/YjsProseMirror';
import './App.css';

function App() {
  return (
    <>
      <Container>
        <Row>
          <Col>
            <h1>Monaco Editor</h1>

            <YjsMonacoEditor />
          </Col>
        </Row>
        <Row>
          <Col>
            <h1>Prose Mirror</h1>

            <YjsProseMirror />
          </Col>
        </Row>
      </Container>
    </>
  )
}

export default App
