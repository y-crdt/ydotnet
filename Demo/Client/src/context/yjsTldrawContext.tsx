import {
    SerializedSchema,
    TLAnyShapeUtilConstructor,
    TLRecord,
    TLStore,
    TLStoreWithStatus,
    createTLStore,
    defaultShapeUtils,
} from '@tldraw/tldraw';
import { createContext, useMemo, useState } from 'react';
import { YKeyValue } from 'y-utility/y-keyvalue';
import * as Y from 'yjs';
import { useYjs } from '../hooks/useYjs';

export interface IYjsTldrawContext {
    readonly store: TLStore;
    readonly yStore: YKeyValue<TLRecord>;
    readonly meta: Y.Map<SerializedSchema>;
    readonly storeWithStatus: TLStoreWithStatus;
    readonly setStoreWithStatus: React.Dispatch<
    React.SetStateAction<TLStoreWithStatus>
    >;
}

export interface IOptions extends React.PropsWithChildren<object> {
    readonly id: string;
    readonly shapeUtils?: TLAnyShapeUtilConstructor[];
}

export const YjsTldrawContextProvider: React.FunctionComponent<IOptions> = ({
    children,
    ...props
}: IOptions) => {
    const { yjsDocument } = useYjs();
    const [store] = useState(() => {
        const store = createTLStore({
            shapeUtils: [...defaultShapeUtils, ...(props?.shapeUtils || [])],
        });
        return store;
    });

    const [storeWithStatus, setStoreWithStatus] = useState<TLStoreWithStatus>({
        status: 'loading',
    });

    const { yStore, meta } = useMemo(() => {
        const yArr = yjsDocument.getArray<{ key: string; val: TLRecord }>(
            `tl_${props.id}`
        );
        const yStore = new YKeyValue(yArr);
        const meta = yjsDocument.getMap<SerializedSchema>('meta');

        return { yStore, meta };
    }, [props?.id, yjsDocument]);

    const contextProps: IYjsTldrawContext = {
        store,
        yStore,
        meta,
        storeWithStatus,
        setStoreWithStatus,
    };

    return (
        <YjsTldrawContext.Provider value={contextProps}>
            {children}
        </YjsTldrawContext.Provider>
    );
};

export const YjsTldrawContext = createContext<IYjsTldrawContext | undefined>(
    undefined
);
