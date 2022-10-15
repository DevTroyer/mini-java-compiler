using static Compiler.Resources;

namespace Compiler {
    class Constant : ISymbolTableEntry {
        public Token Token { get; set; }
        public string Lexeme { get; set; }
        public int Depth { get; set; }
        public EntryType TypeOfEntry { get; set; }
        public DataType TypeOfConst { get; set; }
        public int Offset { get; set; }
        public dynamic Value { get; set; }
        public string OffsetNotation { get; set; }

        public static implicit operator Constant(SymbolTableEntry symbolTableEntry) {
            return new Constant() {
                Lexeme = symbolTableEntry.Lexeme,
                Token = symbolTableEntry.Token,
                Depth = symbolTableEntry.Depth,
                TypeOfEntry = symbolTableEntry.TypeOfEntry,
                OffsetNotation = symbolTableEntry.OffsetNotation
            };
        }
    }
}
