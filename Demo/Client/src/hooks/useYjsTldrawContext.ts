import * as React from 'react';
import { YjsTldrawContext } from '../context/yjsTldrawContext';

export function useYjsTldrawContext() {
    const context = React.useContext(YjsTldrawContext);

    if (context === undefined) {
        throw new Error(
            'useYjsTldrawContext() should be called with the YjsTldrawContext defined.'
        );
    }

    return context;
}
