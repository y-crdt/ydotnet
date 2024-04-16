import { YjsMonacoEditor } from '../components/YjsMonacoEditor';
import { YjsContextProvider } from '../context/yjsContext';

function MonacoPage() {
    return (
        <YjsContextProvider roomName="monaco">
            <h3>Monaco</h3>

            <YjsMonacoEditor />
        </YjsContextProvider>
    );
}

export default MonacoPage;
