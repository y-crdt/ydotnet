import React from 'react';
import { schema } from 'prosemirror-schema-basic';
import { EditorState } from 'prosemirror-state';
import { EditorView } from 'prosemirror-view';
import { keymap } from 'prosemirror-keymap';
import { ySyncPlugin, yCursorPlugin, yUndoPlugin, undo, redo } from 'y-prosemirror';
import { useYjs } from '../hooks/useYjs';

export const YjsProseMirror = () => {
  const { yjsDocument, yjsConnector } = useYjs();
  const yText = yjsDocument.getText('prosemirror');
  const viewHost = React.useRef(null);
  const viewRef = React.useRef(null);

  React.useEffect(() => {
    const state = EditorState.create({
      schema,
      plugins: [
        ySyncPlugin(yText),
        yCursorPlugin(yjsConnector.awareness),
        yUndoPlugin(),
        keymap({
          'Mod-z': undo,
          'Mod-y': redo,
          'Mod-Shift-z': redo
        })
      ]
    });

    viewRef.current = new EditorView(viewHost.current, { state });

    return () => {
      (viewRef.current as any)?.destroy();
    };
  }, [yText, yjsConnector.awareness]);

  return (
    <div ref={viewHost} />
  );
};
