import * as React from 'react';
import { YjsContext } from '../context/yjsContext';

export function useYjs() {
  const yjsContext = React.useContext(YjsContext);

  if (yjsContext === undefined) {
    throw new Error('useYjs() should be called with the YjsContext defined.');
  }

  return yjsContext;
}
