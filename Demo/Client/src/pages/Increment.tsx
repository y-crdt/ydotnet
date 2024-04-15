import { Awareness } from '../components/Awareness';
import { Increment } from '../components/Increment';
import { YjsContextProvider } from '../context/yjsContext';

function IncrementPage() {
    return (
        <YjsContextProvider roomName="increment">
            <h3>Increment</h3>

            <Increment />

            <h3 className='mt-4'>Awareness</h3>

            <Awareness />
        </YjsContextProvider>
    );
}

export default IncrementPage;
