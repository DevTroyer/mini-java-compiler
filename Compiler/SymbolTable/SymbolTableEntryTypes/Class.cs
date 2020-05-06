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
        public List<string> ListOfVariableNames { get; set; } = new List<string>();
        public List<string> ListOfMethodNames { get; set; } = new List<string>();
        public string OffsetNotation { get; set; }

        public static implicit operator Class(SymbolTableEntry symbolTableEntry)
        {
            return new Class()
            {
                Lexeme = symbolTableEntry.Lexeme,
                Token = symbolTableEntry.Token,
                Depth = symbolTableEntry.Depth,
                TypeOfEntry = symbolTableEntry.TypeOfEntry,
                OffsetNotation = symbolTableEntry.OffsetNotation
            };
        }
    }
}
