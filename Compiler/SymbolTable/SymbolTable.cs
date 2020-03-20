// Tucker Troyer
// Compiler Construction
// Assignment 4 - Symbol Table
// Dr. Hamer
// 3/6/2020

using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler
{
    public class SymbolTable : Resources
    {
        const int tableSize = 211;
        public List<ISymbolTableEntry>[] symbolTable { get; set; }

        /// <summary>
        /// Constructor for the Symbol Table
        /// </summary>
        public SymbolTable()
        {
            symbolTable = new List<ISymbolTableEntry>[tableSize];

            for (int i = 0; i < tableSize; i++)
            {
                symbolTable[i] = new List<ISymbolTableEntry>();
            }
        }

        /// <summary>
        /// Inserts the lexemes, token, and depth into a record in the symbol table.
        /// </summary>
        public void Insert(ISymbolTableEntry symbolTableEntry)
        {
            int hashIndex = Hash(symbolTableEntry.Lexeme);

            ISymbolTableEntry entryFoundInSymbolTable = null;

            if (symbolTable[hashIndex].Count > 0 && symbolTable[hashIndex][0].TypeOfEntry == symbolTableEntry.TypeOfEntry && symbolTable[hashIndex].Count % 2 != 0)
            {
                entryFoundInSymbolTable = symbolTable[hashIndex][0];
            }

            if (entryFoundInSymbolTable != null) { symbolTable[hashIndex][0] = symbolTableEntry; }
            else { symbolTable[hashIndex].Insert(0, symbolTableEntry); }
        }

        /// <summary>
        /// Uses the lexeme to find the entry and returns a pointer to that entry.
        /// </summary>
        public ISymbolTableEntry Lookup(string lexeme)
        {
            int hashIndex = Hash(lexeme);

            ISymbolTableEntry entryFoundInSymbolTable = symbolTable[hashIndex].FirstOrDefault(entry => entry.Lexeme == lexeme);

            if (entryFoundInSymbolTable == null)
            {
                ExceptionHandler.ThrowLexemeException(lexeme);
            }

            return entryFoundInSymbolTable;
        }

        /// <summary>
        /// Is passed the depth and deletes all records that are in the table at that depth.
        /// </summary>
        public void DeleteDepth(int depth)
        {
            foreach (List<ISymbolTableEntry> entriesList in symbolTable)
            {
                if (entriesList.Count > 0)
                {
                    entriesList.RemoveAll(entry => entry.Depth == depth);
                }
            }
        }

        /// <summary>
        /// Writes out all variables (lexeme only) that are in the table at a specified depth.
        /// </summary>
        public void WriteTable(int depth)
        {
            Console.WriteLine($"The following lexemes are at depth {depth}:");

            foreach (List<ISymbolTableEntry> entriesList in symbolTable)
            {
                if (entriesList.Count > 0)
                {
                    foreach (ISymbolTableEntry entry in entriesList)
                    {
                        if (entry.Depth == depth)
                        {
                            Console.WriteLine(entry.Lexeme);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Passed a lexemes and returns the location for that lexeme.
        /// </summary>
        private int Hash(string lexeme)
        {
            return Math.Abs(lexeme.GetHashCode()) % tableSize;
        }
    }
}