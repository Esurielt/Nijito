using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UndoableCommands
{
    public delegate TArg DynamicValueGetter<TArg>();  // Slightly more legible than System.Func<TDynamicArg>.
    public abstract class UndoableCommand_DynamicArg<TContext, TSavedInfo, TArg> : UndoableCommand_StaticArg<TContext, TSavedInfo, TArg>
    {
        // Constructor passes back the result of the argument getter at time of construction.
        public UndoableCommand_DynamicArg(TContext context, DynamicValueGetter<TArg> argumentGetter)
                : base(context, argumentGetter.Invoke()) { }   //<-- note the explicit invocation of the argument getter (for clarity)
    }
}