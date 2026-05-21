using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;

namespace MoonSharp.Interpreter.Execution.VM
{
    internal sealed partial class Processor
    {
        private class DebugContext
        {
            public bool DebuggerEnabled = true;
            public IDebugger DebuggerAttached;
            public DebuggerAction.ActionType DebuggerCurrentAction = DebuggerAction.ActionType.None;
            public int DebuggerCurrentActionTarget = -1;
            public SourceRef LastHlRef;
            public int ExStackDepthAtStep = -1;
            public readonly List<SourceRef> BreakPoints = new();
            public bool LineBasedBreakPoints;
        }
    }
}