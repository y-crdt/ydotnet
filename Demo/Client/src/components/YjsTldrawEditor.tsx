import { Tldraw, track, useEditor } from '@tldraw/tldraw';
import '@tldraw/tldraw/tldraw.css';
import { useYjsTldrawStore } from '../hooks/useTldrawStore';

/* 
    see tldraw docs for more info
    https://tldraw.dev/docs/collaboration
    https://github.com/tldraw/tldraw-yjs-example
*/

const NameEditor = track(() => {
    const editor = useEditor();

    const { color, name } = editor.user.getUserPreferences();

    return (
        <div style={{ pointerEvents: 'all', display: 'flex' }}>
            <input
                type="color"
                value={color}
                onChange={(e) => {
                    editor.user.updateUserPreferences({
                        color: e.currentTarget.value,
                    });
                }}
            />
            <input
                value={name}
                onChange={(e) => {
                    editor.user.updateUserPreferences({
                        name: e.currentTarget.value,
                    });
                }}
            />
        </div>
    );
});

const YjsTldrawEditor = (props: { id: string }) => {
    const store = useYjsTldrawStore(props);

    return (
        <Tldraw
            autoFocus
            store={store}
            components={{
                SharePanel: NameEditor,
            }}
        />
    );
};

export default YjsTldrawEditor;
