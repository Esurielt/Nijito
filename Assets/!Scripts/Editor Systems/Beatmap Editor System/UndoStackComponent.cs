using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UndoableCommands;

namespace Editors
{
    public class UndoStackComponent : EditorComponent
    {
        public Button UndoButton;
        public Button RedoButton;

        private List<IUndoableCommand> _undoStack = new List<IUndoableCommand>();
        private int _undoStackPointer = 0;  //<-- points to the number after the last index (the stack count).
                                            //When pointer = undo stack count, then there are no undo actions to be made.
        public override void RegisterHotkeys()
        {
            RegisterHotkey("Undo", () => Undo(), new KeyCombos.KeyCombo(KeyCode.Z, KeyCombos.ToggleKey.Ctrl));
            RegisterHotkey("Redo", () => Redo(), new KeyCombos.KeyCombo(KeyCode.Y, KeyCombos.ToggleKey.Ctrl));
        }
        protected override void SubscribeToEventsInternal()
        {
            UndoButton.onClick.AddListener(Undo);
            RedoButton.onClick.AddListener(Redo);
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            UndoButton.onClick.RemoveAllListeners();
            RedoButton.onClick.RemoveAllListeners();
        }
        protected override void CleanUpInternal()
        {
            _undoStack.Clear();
        }
        public void ExecuteCommand(IUndoableCommand command)
        {
            if (command == null)
                return;

            //if undo stack pointer is before the last index of the list, there have been undo actions made
            if (_undoStackPointer < _undoStack.Count)
            {
                //overwrite remaining undo stack
                _undoStack = _undoStack.GetRange(0, _undoStackPointer); //<-- undo stack becomes the first N actions, where N is the pointer value (convenient!)
            }

            command.Execute();
            Game.Log(Logging.Category.SONG_DATA, $"Editor command executed: {command.Description}.", Logging.Level.LOG);

            _undoStack.Add(command);
            _undoStackPointer++;
        }
        public void Undo()
        {
            //if the undo stack pointer is at 0, then there are no more actions in the undo stack
            if (_undoStackPointer == 0)
            {
                Game.Log(Logging.Category.SONG_DATA, "Nothing to undo!", Logging.Level.LOG);
                return;
            }

            var command = _undoStack[_undoStackPointer - 1];    //<-- undo the last action

            command.Undo();
            _undoStackPointer--;

            Game.Log(Logging.Category.SONG_DATA, $"Editor command undone: {command.Description}.", Logging.Level.LOG);
        }
        public void Redo()
        {
            //if undo stack pointer = the stack count, there are no undo actions to redo. do nothing.
            if (_undoStackPointer == _undoStack.Count)
            {
                Game.Log(Logging.Category.SONG_DATA, "Nothing to redo!", Logging.Level.LOG);
                return;
            }

            var command = _undoStack[_undoStackPointer];    //<-- do the 'next'(current) action

            command.Execute();
            _undoStackPointer++;

            Game.Log(Logging.Category.SONG_DATA, $"Editor command redone: {command.Description}.", Logging.Level.LOG);
        }
    }
}