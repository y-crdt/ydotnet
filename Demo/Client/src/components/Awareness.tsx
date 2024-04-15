import * as React from 'react';
import { Input } from 'reactstrap';
import { useYjs } from '../hooks/useYjs';

export const Awareness = () => {
    const awareness = useYjs().yjsConnector.awareness;
    const [state, setState] = React.useState<unknown>({});

    React.useEffect(() => {
        const updateUsers = () => {
            const allStates: Record<string, unknown> = {};

            awareness.getStates().forEach((value, key) => {
                allStates[key.toString()] = value;
            });

            setState(allStates);
        };

        updateUsers();
        awareness.on('change', updateUsers);
        awareness.setLocalStateField('user', {
            random: Math.random(),
            message: 'Hello',
        });

        console.log(`Current CLIENT ID: ${awareness.clientID}`);

        return () => {
            awareness.off('change', updateUsers);
        };
    }, [awareness]);

    return (
        <Input
            type="textarea"
            readOnly
            value={JSON.stringify(state, undefined, 2)}
        />
    );
};
