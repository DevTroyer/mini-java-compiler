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
                SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer(args[0]);

                syntaxAnalyzer.Prog();

                if (Resources.token == Resources.Token.eoft)
                {
                    Console.WriteLine("Good job! You did it!");
                }
                else
                {
                    Console.WriteLine("Error - Unused tokens");
                }
            }
        }
    }
}