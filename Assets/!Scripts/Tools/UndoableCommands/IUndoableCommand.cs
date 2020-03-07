using System.Collections.Generic;

namespace UndoableCommands
{
    public interface IUndoableCommand
    {
        string Description { get; }
        void Execute();
        void Undo();
        //void Redo(); just execute again, it works.
    }
}
