/* eslint-disable @typescript-eslint/no-explicit-any */
import * as React from 'react';
import * as yMonaco from 'y-monaco';
import MonacoEditor, { monaco } from 'react-monaco-editor';
import { useYjs } from '../hooks/useYjs';

export const YjsMonacoEditor = () => {
    const { yjsDocument, yjsConnector } = useYjs();
    const yText = yjsDocument.getText('monaco');

    const [, setMonacoEditor] = React.useState<monaco.editor.ICodeEditor>();
    const [, setMonacoBinding] = React.useState<yMonaco.MonacoBinding | null>(null);

    const _onEditorDidMount = React.useCallback((editor: monaco.editor.ICodeEditor): void => {
        editor.focus();
        editor.setValue('');

        setMonacoEditor(editor);
        setMonacoBinding(new yMonaco.MonacoBinding(yText, editor.getModel()!, new Set([editor]) as any, yjsConnector.awareness));
    }, [yjsConnector.awareness, yText, setMonacoEditor, setMonacoBinding]);

    return (
        <div>
            <MonacoEditor
                editorDidMount={(e) => _onEditorDidMount(e)}
            />
        </div>
    );
};
