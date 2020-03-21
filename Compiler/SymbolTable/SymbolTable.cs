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
            depth = 0;

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
            return symbolTable[Hash(lexeme)].FirstOrDefault(entry => entry.Lexeme == lexeme);
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

        /// <summary>
        /// Creates a baseline table entry and inserts it into the symbol table.
        /// </summary>
        /// <param name="entryType"></param>
        public ISymbolTableEntry CreateTableEntry(EntryType entryType)
        {
            SymbolTableEntry entry = new SymbolTableEntry(lexemes.ToString(), Token.idt, depth, entryType);
            CheckDuplicates();
            Insert(entry);

            return entry;
        }

        /// <summary>
        /// Checks the symbol table for a duplicate entry at the same depth.
        /// </summary>
        /// <returns></returns>
        private void CheckDuplicates()
        {
            int hashIndex = Hash(lexemes.ToString());

            ISymbolTableEntry duplicateEntryFound = symbolTable[hashIndex].FirstOrDefault(duplicate => duplicate.Lexeme == lexemes.ToString() && duplicate.Depth == depth);

            if (duplicateEntryFound != null)
            {
                ExceptionHandler.ThrowDuplicateEntryException(lexemes.ToString());
            }
        }

        /// <summary>
        /// Method that converts a table entry to a class entry.
        /// </summary>
        /// <param name="tableEntry"></param>
        public void ConvertEntryToClassEntry(ISymbolTableEntry tableEntry)
        {
            Class entry = Lookup(tableEntry.Lexeme) as SymbolTableEntry;
            entry.SizeOfLocalVariables = sizeOfLocalVariables;
            entry.ListOfVariableNames = listOfVariableNames;
            entry.ListOfMethodNames = listOfMethodNames;
            Insert(entry);
        }

        /// <summary>
        /// Method that converts a table entry to a const entry.
        /// </summary>
        /// <param name="tableEntry"></param>
        public void ConvertEntryToConstEntry(ISymbolTableEntry tableEntry)
        {
            //Constant entry = Lookup(tableEntry.Lexeme) as SymbolTableEntry;
            //Insert(entry);
        }

        /// <summary>
        /// Method that converts a table entry to a var entry.
        /// </summary>
        /// <param name="tableEntry"></param>
        public void ConvertEntryToVarEntry(ISymbolTableEntry tableEntry)
        {
            Variable entry = Lookup(tableEntry.Lexeme) as SymbolTableEntry;
            Insert(entry);
        }

        /// <summary>
        /// Method that converts a table entry to a method entry.
        /// </summary>
        /// <param name="tableEntry"></param>
        public void ConvertEntryToMethodEntry(ISymbolTableEntry tableEntry)
        {
            Method entry = Lookup(tableEntry.Lexeme) as SymbolTableEntry;
            Insert(entry);
        }
    }
}