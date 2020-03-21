using System;
using static Compiler.Resources;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            //SymbolTable symbolTable = new SymbolTable();

            //// Create baseline entry
            //SymbolTableEntry testEntryOne = new SymbolTableEntry("testVariableOne", Token.idt, 1, EntryType.varEntry);

            //// Insert baseline entry
            //symbolTable.Insert(testEntryOne);

            //// Lookup baseline entry and 'initialize' variable
            //Variable testVariableOne = symbolTable.Lookup("testVariableOne") as SymbolTableEntry;

            //// Assign variable specs
            //testVariableOne.TypeOfVariable = VarType.intType;
            //testVariableOne.Offset = 4;
            //testVariableOne.Size = 6;

            //// Insert variable in place of baseline entry
            //symbolTable.Insert(testVariableOne);

            //// More tests
            //SymbolTableEntry testEntryTwo = new SymbolTableEntry("testVariableTwo", Token.idt, 1, EntryType.varEntry);
            //symbolTable.Insert(testEntryTwo);
            //Variable testVariableTwo = symbolTable.Lookup("testVariableTwo") as SymbolTableEntry;
            //testVariableTwo.TypeOfVariable = VarType.booleanType;
            //testVariableTwo.Offset = 6;
            //testVariableTwo.Size = 10;
            //symbolTable.Insert(testVariableTwo);

            //symbolTable.WriteTable(1);

            //SymbolTableEntry testentryThree = new SymbolTableEntry("testMethodOne", Token.idt, 1, EntryType.methodEntry);
            //symbolTable.Insert(testentryThree);
            //Method testMethodOne = symbolTable.Lookup("testMethodOne") as SymbolTableEntry;
            //testMethodOne.NumOfParams = 2;
            //testMethodOne.ParamPassingMode.Add(ParameterPassingMode.passByValue);
            //testMethodOne.ReturnType = VarType.voidType;
            //symbolTable.Insert(testMethodOne);

            //SymbolTableEntry testEntryFour = new SymbolTableEntry("testConstOne", Token.idt, 1, EntryType.constEntry);
            //symbolTable.Insert(testEntryFour);
            //Constant<int> testConstOne = symbolTable.Lookup("testConstOne") as SymbolTableEntry;
            //testConstOne.Value = 10;
            //testConstOne.Offset = 16;
            //symbolTable.Insert(testConstOne);

            //symbolTable.WriteTable(1);

            //symbolTable.DeleteDepth(1);

            //symbolTable.WriteTable(1);

            //SymbolTableEntry testEntryFive = new SymbolTableEntry("testClassOne", Token.idt, 2, EntryType.classEntry);
            //symbolTable.Insert(testEntryFive);
            //Class testClassOne = symbolTable.Lookup("testClassOne") as SymbolTableEntry;
            //testClassOne.SizeOfLocalVariables = 16;
            //testClassOne.ListOfMethodNames.Add("methodOne");
            //symbolTable.Insert(testClassOne);

            //symbolTable.WriteTable(2);
            //symbolTable.DeleteDepth(2);
            //symbolTable.WriteTable(2);

            //ISymbolTableEntry testEntrySix = symbolTable.Lookup("pizza");

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: filename is not entered");
                return;
            }
            else
            {
                SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer(args[0]);

                syntaxAnalyzer.Prog();

                if (token == Token.eoft)
                {
                    Console.WriteLine("Good job! You did it!");
                }
                else
                {
                    ExceptionHandler.ThrowUnusedTokensException();
                }
            }
        }
    }
}