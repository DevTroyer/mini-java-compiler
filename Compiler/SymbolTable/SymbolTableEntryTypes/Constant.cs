using static Compiler.Resources;

namespace Compiler
{
    class Constant<DataType> : ISymbolTableEntry
    {
        public Token Token { get; set; }
        public string Lexeme { get; set; }
        public int Depth { get; set; }
        public EntryType TypeOfEntry { get; set; }
        public ConstType TypeOfConst { get; set; }
        public int Offset { get; set; }
        public DataType Value { get; set; }

        public static implicit operator Constant<DataType>(SymbolTableEntry symbolTableEntry)
        {
            return new Constant<DataType>()
            {
                Lexeme = symbolTableEntry.Lexeme,
                Token = symbolTableEntry.Token,
                Depth = symbolTableEntry.Depth,
                TypeOfEntry = symbolTableEntry.TypeOfEntry
            };
        }
    }
}
