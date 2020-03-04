using System.Collections.Generic;
using static Compiler.Resources;

namespace Compiler
{
    class Class : ISymbolTableEntry
    {
        public Token Token { get; set; }
        public string Lexeme { get; set; }
        public int Depth { get; set; }
        public EntryType TypeOfEntry { get; set; }
        public int SizeOfLocalVariables { get; set; }
        public List<string> ListOfVariableNames { get; set; }
        public List<string> ListOfMethodNames { get; set; }
    }
}
