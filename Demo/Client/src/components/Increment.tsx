import * as React from 'react';
import { Button, Col, Input, Row } from 'reactstrap';
import { useYjs } from '../hooks/useYjs';

export const Increment = () => {
    const { yjsDocument } = useYjs();
    const map = yjsDocument.getMap('increment');
    const [state, setState] = React.useState<number>(0);

    React.useEffect(() => {
        const handler = () => {
            setState(map.get('value') as number || 0);
        };

        handler();
        map.observeDeep(handler);

        return () => {
            map.unobserveDeep(handler);
        };
    }, [map]);

    React.useEffect(() => {
        yjsDocument.transact(() => {
            map.set('value', state);
        });
    }, [map, state, yjsDocument]);

    const _increment = () => {
        setState(v => v + 1);
    };

    const _decrement = () => {
        setState(v => v - 1);
    };

    return (
        <Row className='gap-0'>
            <Col xs='auto'>
                <Button onClick={_decrement}>-1</Button>
            </Col>
            <Col xs={5} className='pl-2 pr-2'>
                <Input readOnly value={state}></Input>
            </Col>
            <Col xs='auto'>
                <Button onClick={_increment}>+1</Button>
            </Col>
        </Row>
    );
};