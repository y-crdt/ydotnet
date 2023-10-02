import * as React from 'react';
import * as Y from 'yjs';
import { WebsocketProvider } from 'y-websocket';

export interface IYjsContext {
  readonly yjsDocument: Y.Doc;
  readonly yjsConnector: WebsocketProvider;
}

export interface IOptions extends React.PropsWithChildren<{}> {
  readonly baseUrl: string;
}

export const YjsContextProvider: React.FunctionComponent<IOptions> = (props: IOptions) => {
  const { baseUrl } = props;

  const contextProps: IYjsContext = React.useMemo(() => {
    const yjsDocument = new Y.Doc();
    const yjsConnector = new WebsocketProvider(baseUrl, 'test', yjsDocument);

    return { yjsDocument, yjsConnector };
  }, [baseUrl]);

  return <YjsContext.Provider value={contextProps}>{props.children}</YjsContext.Provider>;
};

export const YjsContext = React.createContext<IYjsContext | undefined>(undefined);
