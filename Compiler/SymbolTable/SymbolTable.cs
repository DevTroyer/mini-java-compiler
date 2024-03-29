﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler {
    public class SymbolTable : Resources {
        const int tableSize = 211;
        public List<ISymbolTableEntry>[] symbolTable { get; set; }

        /// <summary>
        /// Constructor for the Symbol Table
        /// </summary>
        public SymbolTable() {
            symbolTable = new List<ISymbolTableEntry>[tableSize];
            depth = 0;

            for (int i = 0; i < tableSize; i++) {
                symbolTable[i] = new List<ISymbolTableEntry>();
            }
        }

        /// <summary>
        /// Inserts the lexemes, token, and depth into a record in the symbol table.
        /// </summary>
        public void Insert(ISymbolTableEntry symbolTableEntry) {
            uint hashIndex = Hash(symbolTableEntry.Lexeme);

            if (symbolTableEntry.TypeOfEntry != EntryType.tableEntry) {
                int index = symbolTable[hashIndex].FindIndex(tableEntry => tableEntry.Lexeme == symbolTableEntry.Lexeme && tableEntry.Depth == symbolTableEntry.Depth);
                symbolTable[hashIndex][index] = symbolTableEntry;
            } else {
                symbolTable[hashIndex].Insert(0, symbolTableEntry);
            }
        }

        /// <summary>
        /// Uses the lexeme to find the entry and returns a pointer to that entry.
        /// </summary>
        public ISymbolTableEntry Lookup(string lexeme) {
            return symbolTable[Hash(lexeme)].FirstOrDefault(entry => entry.Lexeme == lexeme);
        }

        /// <summary>
        /// Is passed the depth and deletes all records that are in the table at that depth.
        /// </summary>
        public void DeleteDepth(int depth) {
            foreach (List<ISymbolTableEntry> entriesList in symbolTable) {
                if (entriesList.Count > 0) {
                    entriesList.RemoveAll(entry => entry.Depth == depth);
                }
            }
        }

        /// <summary>
        /// Writes out all variables (lexeme only) that are in the table at a specified depth.
        /// </summary>
        public void WriteTable(int depth) {
            Console.WriteLine($"The following lexemes are at depth {depth}:");

            foreach (List<ISymbolTableEntry> entriesList in symbolTable) {
                if (entriesList.Count > 0) {
                    foreach (ISymbolTableEntry entry in entriesList) {
                        if (entry.Depth == depth) {
                            Console.WriteLine($"Lexeme: {entry.Lexeme}");
                            Console.WriteLine($"Type of Entry: {entry.TypeOfEntry}");
                        }
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Passed a lexemes and returns the location for that lexeme.
        /// </summary>
        private uint Hash(string lexeme) {
            uint hash = 0, g;

            foreach (char c in lexeme) {
                hash = (hash << 4) + (byte)c;

                if ((g = hash & 0xf0000000) != 0) {
                    hash ^= (g >> 24);
                    hash ^= g;
                }
            }

            return hash % tableSize;
        }

        /// <summary>
        /// Creates a baseline table entry and inserts it into the symbol table.
        /// </summary>
        /// <param name="entryType"></param>
        public ISymbolTableEntry CreateTableEntry(EntryType entryType) {
            SymbolTableEntry entry = new SymbolTableEntry(Token.idt, lexeme.ToString(), depth, entryType);
            CheckDuplicates();
            Insert(entry);

            return entry;
        }

        /// <summary>
        /// Checks the symbol table for a duplicate entry at the same depth.
        /// </summary>
        /// <returns></returns>
        private void CheckDuplicates() {
            ISymbolTableEntry duplicateEntryFound = symbolTable[Hash(lexeme.ToString())].FirstOrDefault(duplicate => duplicate.Lexeme == lexeme.ToString() && duplicate.Depth == depth);

            if (duplicateEntryFound != null) {
                ExceptionHandler.ThrowDuplicateIdentifierException(lexeme.ToString());
            }
        }

        /// <summary>
        /// Method that converts a table entry to a class entry.
        /// </summary>
        /// <param name="tableEntry"></param>
        public void ConvertEntryToClassEntry(ISymbolTableEntry tableEntry) {
            Class entry = tableEntry as SymbolTableEntry;
            entry.SizeOfLocalVariables = sizeOfLocalVariables;
            entry.ListOfVariableNames = listOfVariableNames;
            entry.ListOfMethodNames = listOfMethodNames;
            entry.TypeOfEntry = EntryType.classEntry;
            Insert(entry);
            /* Use for testing: DisplayClassEntry(entry); */
        }

        /// <summary>
        /// Method that converts a table entry to a const double entry.
        /// </summary>
        /// <param name="tableEntry"></param>
        public void ConvertEntryToConstantEntry(ISymbolTableEntry tableEntry, dynamic value) {
            Constant entry = tableEntry as SymbolTableEntry;
            entry.Value = value;
            entry.TypeOfConst = dataType;
            entry.Offset = offset;
            entry.TypeOfEntry = EntryType.constEntry;
            entry.OffsetNotation = BpOffsetNotation;
            Insert(entry);
            /* Use for testing: DisplayConstDoubleEntry(entry); */
        }
        
        /// <summary>
        /// Method that converts a table entry to a var entry.
        /// </summary>
        /// <param name="tableEntry"></param>
        public void ConvertEntryToVariableEntry(IntermediateCodeGenerator intermediateCodeGenerator, ISymbolTableEntry tableEntry, bool isParameter) {
            Variable entry = tableEntry as SymbolTableEntry;
            entry.TypeOfVariable = dataType;
            entry.Offset = offset;
            entry.OffsetNotation = BpOffsetNotation;//isParameter ? intermediateCodeGenerator.CalculateParameterOffsetNotation(entry) : intermediateCodeGenerator.CalculateLocalVariableOffsetNotation(entry);
            entry.Size = size;
            entry.TypeOfEntry = EntryType.varEntry;
            entry.OffsetNotation = BpOffsetNotation;
            Insert(entry);
            /* Use for testing: DisplayVariableEntry(entry); */
        }

        /// <summary>
        /// Method that converts a table entry to a method entry.
        /// </summary>
        /// <param name="tableEntry"></param>
        public void ConvertEntryToMethodEntry(ISymbolTableEntry tableEntry) {
            Method entry = tableEntry as SymbolTableEntry;
            entry.SizeOfLocalVariables = sizeOfLocalMethodVariables;
            entry.SizeOfFormalParameters = sizeOfFormalParameters;
            entry.NumberOfParameters = numOfParameters;
            entry.ReturnType = returnType;
            entry.ParameterType = parameterType;
            entry.TypeOfEntry = EntryType.methodEntry;
            Insert(entry);
            /* Use for testing: DisplayMethodEntry(entry); */
        }

        #region Testing Purposes

        private void DisplayVariableEntry(Variable entry) {
            Console.WriteLine("Variable");
            Console.WriteLine($"Token: {entry.Token}");
            Console.WriteLine($"Lexeme: {entry.Lexeme}");
            Console.WriteLine($"Depth: {entry.Depth}");
            Console.WriteLine($"Type of entry: {entry.TypeOfEntry}");
            Console.WriteLine($"Type of variable: {entry.TypeOfVariable}");
            Console.WriteLine($"Offset: {entry.Offset}");
            Console.WriteLine($"Size: {entry.Size}");
            Console.WriteLine();
        }

        private void DisplayConstantEntry(Constant entry) {
            Console.WriteLine("Constant");
            Console.WriteLine($"Token: {entry.Token}");
            Console.WriteLine($"Lexeme: {entry.Lexeme}");
            Console.WriteLine($"Depth: {entry.Depth}");
            Console.WriteLine($"Type of entry: {entry.TypeOfEntry}");
            Console.WriteLine($"Type of const: {entry.TypeOfConst}");
            Console.WriteLine($"Offset: {entry.Offset}");
            Console.WriteLine($"Value: {entry.Value}");
            Console.WriteLine();
        }

        private void DisplayMethodEntry(Method entry) {
            Console.WriteLine("Method");
            Console.WriteLine($"Token: {entry.Token}");
            Console.WriteLine($"Lexeme: {entry.Lexeme}");
            Console.WriteLine($"Depth: {entry.Depth}");
            Console.WriteLine($"Type of entry: {entry.TypeOfEntry}");
            Console.WriteLine($"Size of local variables: {entry.SizeOfLocalVariables}");
            Console.WriteLine($"Size of formal parameters: {entry.SizeOfFormalParameters}");
            Console.WriteLine($"Number of parameters: {entry.NumberOfParameters}");
            Console.WriteLine($"Return type: {entry.ReturnType}");
            Console.Write($"Parameter type: ");
            foreach (DataType parameterType in entry.ParameterType) {
                Console.Write($"{parameterType} ");
            }
            Console.WriteLine("\n");
        }

        private void DisplayClassEntry(Class entry) {
            Console.WriteLine("Class");
            Console.WriteLine($"Token: {entry.Token}");
            Console.WriteLine($"Lexeme: {entry.Lexeme}");
            Console.WriteLine($"Depth: {entry.Depth}");
            Console.WriteLine($"Type of entry: {entry.TypeOfEntry}");
            Console.WriteLine($"Size of local variables: {entry.SizeOfLocalVariables}");
            Console.Write($"List of local variable names: ");
            foreach (string variableName in entry.ListOfVariableNames) {
                Console.Write($"{variableName} ");
            }
            Console.Write($"\nList of local method names: ");
            foreach (string methodName in entry.ListOfMethodNames) {
                Console.Write($"{methodName} ");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        #endregion
    }
}