using System.Collections.Generic;

namespace MoonSharp.Interpreter.Execution
{
    internal class RuntimeScopeFrame
    {
        public List<SymbolRef> DebugSymbols { get; }

        public int Count { get => DebugSymbols.Count; }

        public int ToFirstBlock { get; internal set; }

        public RuntimeScopeFrame()
        {
            DebugSymbols = new List<SymbolRef>();
        }

        public override string ToString()
        {
            return string.Format("ScopeFrame : #{0}", Count);
        }
    }
}