using static Compiler.Resources;

namespace Compiler {
    public class Variable : ISymbolTableEntry {
        public Token Token { get; set; }
        public string Lexeme { get; set; }
        public int Depth { get; set; }
        public EntryType TypeOfEntry { get; set; }
        public DataType TypeOfVariable { get; set; }
        public string OffsetNotation { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }

        public static implicit operator Variable(SymbolTableEntry symbolTableEntry) {
            return new Variable() {
                Lexeme = symbolTableEntry.Lexeme,
                Token = symbolTableEntry.Token,
                Depth = symbolTableEntry.Depth,
                TypeOfEntry = symbolTableEntry.TypeOfEntry,
                OffsetNotation = symbolTableEntry.OffsetNotation
            };
        }
    }
}
