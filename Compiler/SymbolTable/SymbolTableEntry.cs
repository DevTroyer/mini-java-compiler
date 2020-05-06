using static Compiler.Resources;

namespace Compiler
{
    public class SymbolTableEntry : ISymbolTableEntry
    {
        public Token Token { get; set; }
        public string Lexeme { get; set; }
        public int Depth { get; set; }
        public EntryType TypeOfEntry { get; set; }
        public string OffsetNotation { get; set; }

        public SymbolTableEntry(Token _token, string _lexeme, int _depth, EntryType _typeOfEntry)
        {
            Token = _token;
            Lexeme = _lexeme;
            Depth = _depth;
            TypeOfEntry = _typeOfEntry;
            OffsetNotation = "";
        }
    }
}
