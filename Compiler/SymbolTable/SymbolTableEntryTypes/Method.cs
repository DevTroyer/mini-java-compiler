using System.Collections.Generic;
using static Compiler.Resources;

namespace Compiler {
    class Method : ISymbolTableEntry {
        public Token Token { get; set; }
        public string Lexeme { get; set; }
        public int Depth { get; set; }
        public EntryType TypeOfEntry { get; set; }
        public int SizeOfLocalVariables { get; set; }
        public int SizeOfFormalParameters { get; set; }
        public int NumberOfParameters { get; set; }
        public DataType ReturnType { get; set; }
        public List<DataType> ParameterType { get; set; } = new List<DataType>();
        public string OffsetNotation { get; set; }

        public static implicit operator Method(SymbolTableEntry symbolTableEntry) {
            return new Method() {
                Lexeme = symbolTableEntry.Lexeme,
                Token = symbolTableEntry.Token,
                Depth = symbolTableEntry.Depth,
                TypeOfEntry = symbolTableEntry.TypeOfEntry,
                OffsetNotation = symbolTableEntry.OffsetNotation
            };
        }
    }
}
