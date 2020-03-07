using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UndoableCommands
{
    public abstract class UndoableCommand_StaticArg<TContext, TSavedInfo, TArg> : UndoableCommand<TContext, TSavedInfo>
    {
        protected TArg Argument { get; private set; }
        public UndoableCommand_StaticArg(TContext context, TArg argument)
            : base(context)
        {
            Argument = argument;
        }
    }
}