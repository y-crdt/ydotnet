import { YjsProseMirror } from '../components/YjsProseMirror';
import { YjsContextProvider } from '../context/yjsContext';

function ProseMirrorPage() {
    return (
        <YjsContextProvider roomName="prosemirror">
            <h3>Prose Mirror</h3>

            <YjsProseMirror />
        </YjsContextProvider>
    );
}

export default ProseMirrorPage;
