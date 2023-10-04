import * as React from 'react';
import { Button, Col, Input, Row } from 'reactstrap';
import { useYjs } from '../hooks/useYjs';

interface Comment {
    text: string;
}

export const Chat = ({ isReadonly }: { isReadonly: boolean }) => {
    const { yjsDocument } = useYjs();
    const array = yjsDocument.getArray<Comment>('stream');
    const [state, setState] = React.useState<Comment[]>([]);
    const [text, setText] = React.useState('');

    React.useEffect(() => {
        const handler = () => {
            setState(array.toJSON());
        };

        array.observeDeep(handler);

        return () => {
            array.unobserveDeep(handler);
        };
    }, [array]);

    const _comment = () => {
        yjsDocument.transact(() => {
            array.push([{ text }]);
        });

        setText('');
    };

    const _setText = (event: React.ChangeEvent<HTMLInputElement>) => {
        setText(event.target.value);
    };

    return (
        <>
            <div className='chat'>
                {state.map((row, i) =>
                    <div key={i}>
                        {row.text}
                    </div>
                )}
            </div>

            {!isReadonly &&
                <Row noGutters>
                    <Col>
                        <Input value={text} onChange={_setText}></Input>
                    </Col>
                    <Col xs='auto' className='pl-2'>
                        <Button onClick={_comment}>Comment</Button>
                    </Col>
                </Row>
            }
        </>
    );
};