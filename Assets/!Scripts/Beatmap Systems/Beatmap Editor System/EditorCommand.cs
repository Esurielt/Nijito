using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap.Editor.EditorCommands
{
    public interface IUndoableCommand
    {
        void Execute();
        void Undo();
    }
    public abstract class EditorCommand : IUndoableCommand
    {
        private readonly BeatmapEditor _editor;
        public EditorCommand(BeatmapEditor editor)
        {
            _editor = editor;
        }
        public void Execute()
        {
            ExecuteInWriter(_editor.BeatmapWriter);
            ExecuteInSceneView(_editor);
        }
        protected abstract void ExecuteInWriter(BeatmapWriter writer);
        protected abstract void ExecuteInSceneView(BeatmapEditor editor);

        public void Undo()
        {
            UndoInWriter(_editor.BeatmapWriter);
            UndoInSceneView(_editor);
        }
        protected abstract void UndoInWriter(BeatmapWriter writer);
        protected abstract void UndoInSceneView(BeatmapEditor editor);
    }
    public class EditorCommand_AdHoc : EditorCommand
    {
        private readonly System.Action<BeatmapEditor> _executeInSceneView;
        private readonly System.Action<BeatmapWriter> _executeInWriter;
        private readonly System.Action<BeatmapEditor> _undoInSceneView;
        private readonly System.Action<BeatmapWriter> _undoInWriter;

        public EditorCommand_AdHoc(BeatmapEditor editor,
            System.Action<BeatmapEditor> executeInSceneView,
            System.Action<BeatmapWriter> executeInWriter,
            System.Action<BeatmapEditor> undoInSceneView,
            System.Action<BeatmapWriter> undoInWriter)
            :base(editor)
        {
            _executeInSceneView = executeInSceneView;
            _executeInWriter = executeInWriter;
            _undoInSceneView = undoInSceneView;
            _undoInWriter = undoInWriter;
        }
        protected override void ExecuteInSceneView(BeatmapEditor editor) => _executeInSceneView(editor);

        protected override void ExecuteInWriter(BeatmapWriter writer) => _executeInWriter(writer);

        protected override void UndoInSceneView(BeatmapEditor editor) => _undoInSceneView(editor);

        protected override void UndoInWriter(BeatmapWriter writer) => _undoInWriter(writer);
    }
    public class EditorCommand_StaticArg<TStaticArg> : EditorCommand_AdHoc
    {
        protected readonly TStaticArg _argument;
        public EditorCommand_StaticArg(BeatmapEditor editor, TStaticArg argument,
            System.Action<BeatmapEditor, TStaticArg> executeInSceneView,
            System.Action<BeatmapWriter, TStaticArg> executeInWriter,
            System.Action<BeatmapEditor, TStaticArg> undoInSceneView,
            System.Action<BeatmapWriter, TStaticArg> undoInWriter)
            :base(editor,
                 ed => executeInSceneView(ed, argument),
                 wr => executeInWriter(wr, argument),
                 ed => undoInSceneView(ed, argument),
                 wr => undoInWriter(wr, argument) )
        {
            _argument = argument;
        }
    }
    public class EditorCommand_DynamicArg<TDynamicArg> : EditorCommand_StaticArg<System.Func<TDynamicArg>>
    {
        public EditorCommand_DynamicArg(BeatmapEditor editor, System.Func<TDynamicArg> dynamicArg,
            System.Action<BeatmapEditor, TDynamicArg> executeInSceneView,
            System.Action<BeatmapWriter, TDynamicArg> executeInWriter,
            System.Action<BeatmapEditor, TDynamicArg> undoInSceneView,
            System.Action<BeatmapWriter, TDynamicArg> undoInWriter
            )
            :base(editor, dynamicArg,
                 (ed,func) => executeInSceneView(ed, func.Invoke()),
                 (wr,func) => executeInWriter(wr, func.Invoke()),
                 (ed, func) => undoInSceneView(ed, func.Invoke()),
                 (wr, func) => undoInWriter(wr, func.Invoke()) )
        { }
    }
    public class AddBlankBeatsAtEndCommand : EditorCommand_DynamicArg<int>
    {
        public AddBlankBeatsAtEndCommand(BeatmapEditor editor, Func<int> getBeatCount)
            :base(editor, getBeatCount,
                  executeInSceneView: (ed, value) => ed.AddBlankBeatsAtEnd(value),
                  executeInWriter: (wr, value) => wr.AddBlankBeatsAtEnd(value),
                  undoInSceneView: (ed, value) => ed.RemoveBeatsFromEnd(value),
                  undoInWriter: (wr, value) => wr.RemoveBeatsFromEnd(value) )
        { }
    }
}
