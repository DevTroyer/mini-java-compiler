using static Compiler.Resources;

namespace Compiler
{
    class Variable : ISymbolTableEntry
    {
        public Token Token { get; set; }
        public string Lexeme { get; set; }
        public int Depth { get; set; }
        public EntryType TypeOfEntry { get; set; }
        public VarType TypeOfVariable { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }

        public Variable(Token _token, string _lexeme, int _depth)
        {
            Token = _token;
            Lexeme = _lexeme;
            Depth = _depth;
        }
    }
}
