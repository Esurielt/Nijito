using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UndoableCommands
{
    public class UndoableCommand_AdHoc<TContext, TSavedInfo> : UndoableCommand<TContext, TSavedInfo>
    {
        private readonly System.Func<TContext, TSavedInfo> _executeInContext;
        private readonly System.Action<TContext, TSavedInfo> _undoInContext;

        private readonly string _description;
        public override string Description => _description;

        public UndoableCommand_AdHoc(TContext context, string description,
            System.Func<TContext, TSavedInfo> executeInContext,
            System.Action<TContext, TSavedInfo> undoInContext)
            : base(context)
        {
            _executeInContext = executeInContext;
            _undoInContext = undoInContext;
            _description = description;
        }
        protected override TSavedInfo ExecuteInContext(TContext context) => _executeInContext(context);
        protected override void UndoInContext(TContext context, TSavedInfo savedInfo) => _undoInContext(context, savedInfo);
    }
}