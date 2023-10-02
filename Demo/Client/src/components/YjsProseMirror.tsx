import * as React from 'react';
import { schema } from 'prosemirror-schema-basic';
import { EditorState } from 'prosemirror-state';
import { EditorView } from 'prosemirror-view';
import { keymap } from 'prosemirror-keymap';
import { ySyncPlugin, yCursorPlugin, yUndoPlugin, undo, redo } from 'y-prosemirror';
import { useYjs } from '../hooks/useYjs';

export const YjsProseMirror = () => {
  const { yjsDocument, yjsConnector } = useYjs();
  const yText = yjsDocument.getXmlFragment('prosemirror');
  const viewHost = React.useRef(null);
  const viewRef = React.useRef<EditorView | null>(null);

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

    const editor = new EditorView(viewHost.current, { state });

    viewRef.current = editor;

    return () => {
      editor.destroy();
    };
  }, [yText, yjsConnector.awareness]);

  return (
    <div ref={viewHost} />
  );
};
