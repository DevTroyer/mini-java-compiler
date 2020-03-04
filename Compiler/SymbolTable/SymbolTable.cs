// Tucker Troyer
// Compiler Construction
// Assignment 4 - Symbol Table
// Dr. Hamer
// 3/6/2020

using System.Collections.Generic;

namespace Compiler
{
    public enum VarType { booleanType, floatType, intType, voidType };
    public enum EntryType { classEntry, methodEntry, varEntry, constEntry };
    public enum ParameterPassingMode { passByValue, passByReference };

    public class SymbolTable : Resources
    {
        const int tableSize = 211;
        public List<ISymbolTableEntry>[] symbolTable { get; set; }

        public SymbolTable()
        {
            symbolTable = new List<ISymbolTableEntry>[tableSize];
        }

        /// <summary>
        /// Inserts the lexemes, token, and depth into a record in the symbol table.
        /// </summary>
        public void insert(string lexemes, Token token, int depth)
        {

        }

        /// <summary>
        /// Uses the lexeme to find the entry and returns a pointer to that entry. 
        /// </summary>
        public void lookup(string lexemes)
        {

        }

        /// <summary>
        /// Is passed the depth and deletes all records that are in the table at that depth.
        /// </summary>
        public void deleteDepth(int depth)
        {

        }

        /// <summary>
        /// Writes out all variables (lexeme only) that are in the table at a specified depth.
        /// </summary>
        public void writeTable(int depth)
        {

        }

        /// <summary>
        /// Passed a lexemes and returns the location for that lexeme.
        /// </summary>
        private void hash(string lexemes)
        {

        }
    }
}