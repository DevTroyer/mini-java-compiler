using System.Collections.Generic;
using static Compiler.Resources;

namespace Compiler
{
    class Method : ISymbolTableEntry
    {
        public Token Token { get; set; }
        public string Lexeme { get; set; }
        public int Depth { get; set; }
        public EntryType TypeOfEntry { get; set; }
        public int SizeOfLocalVariables { get; set; }
        public int NumOfParams { get; set; }
        public VarType ReturnType { get; set; }
        public List<ParameterPassingMode> ParameterPassingMode { get; set; }
        public List<VarType> ParameterType { get; set; }

        public Method(Token _token, string _lexeme, int _depth)
        {
            Token = _token;
            Lexeme = _lexeme;
            Depth = _depth;
        }
    }
}
