using System;
using static Compiler.Resources;

namespace Compiler {
    class Program {
        static void Main(string[] args) {
            if (args.Length != 1) {
                Console.WriteLine("Usage: filename is not entered");
                return;
            } else {
                SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer(args[0]);
                syntaxAnalyzer.Prog();

                if (token == Token.eoft) {
                    Console.WriteLine("Mini Java compiler has successfully finished compiling.");
                } else {
                    ExceptionHandler.ThrowUnusedTokensException();
                }
            }
        }
    }
}