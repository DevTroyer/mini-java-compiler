using System;
using System.Collections.Generic;
using System.Text;
using static Compiler.Resources;

namespace Compiler
{
    class SymbolTableEntry : ISymbolTableEntry
    {
        public Token Token { get; set; }
        public string Lexeme { get; set; }
        public int Depth { get; set; }
        public EntryType TypeOfEntry { get; set; }

        public SymbolTableEntry(Token _token, string _lexeme, int _depth)
        {
            Token = _token;
            Lexeme = _lexeme;
            Depth = _depth;
        }
    }
}
