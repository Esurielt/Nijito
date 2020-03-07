using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UndoableCommands
{
    public abstract class UndoableCommand<TContext> : IUndoableCommand
    {
        protected TContext Context { get; private set; }
        public abstract string Description { get; }
        public UndoableCommand(TContext context)
        {
            Context = context;
        }
        public abstract void Execute();
        public abstract void Undo();
    }

    public abstract class UndoableCommand<TContext, TSavedInfo> : UndoableCommand<TContext>
    {
        protected TSavedInfo SavedInfo { get; private set; }
        public UndoableCommand(TContext context)
            : base(context) { }
        public override void Execute()
        {
            SavedInfo = ExecuteInContext(Context);
        }
        protected abstract TSavedInfo ExecuteInContext(TContext context);    //return saved info

        public override void Undo()
        {
            UndoInContext(Context, SavedInfo);
        }
        protected abstract void UndoInContext(TContext context, TSavedInfo savedInfo);
    }
}