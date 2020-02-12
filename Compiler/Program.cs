using System;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: filename is not entered");
                return;
            }
            else
            {
                LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer(args[0]);

                Console.WriteLine("{0,-8}{1,-12}{2,-40}{3,-14}{4,-14}{5,-40}\n", "Line #:", "Token:", "Lexeme(s):", "Value:", "ValueR:", "Literal:");

                while (Resources.token != Resources.Token.eoft)
                {
                    lexicalAnalyzer.GetNextToken();
                    lexicalAnalyzer.DisplayToken();
                }
            }
        }
    }
}
