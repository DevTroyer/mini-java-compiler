﻿using System.Collections.Generic;
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
        public List<ParameterPassingMode> ParamPassingMode { get; set; } = new List<ParameterPassingMode>();
        public List<VarType> ParameterType { get; set; } = new List<VarType>();

        public static implicit operator Method(SymbolTableEntry symbolTableEntry)
        {
            return new Method()
            {
                Lexeme = symbolTableEntry.Lexeme,
                Token = symbolTableEntry.Token,
                Depth = symbolTableEntry.Depth,
                TypeOfEntry = symbolTableEntry.TypeOfEntry
            };
        }
    }
}