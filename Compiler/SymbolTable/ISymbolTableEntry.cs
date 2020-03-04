using static Compiler.Resources;

namespace Compiler
{
    public interface ISymbolTableEntry
    {
        public Token Token { get; set; }
        public string Lexeme { get; set; }
        public int Depth { get; set; }
        public EntryType TypeOfEntry { get; set; }
    }
}
