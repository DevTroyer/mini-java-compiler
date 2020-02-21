using System;

namespace Compiler
{
    class ExceptionHandler : Resources
    {
        public static void ThrowException(Token expectedToken)
        {
            if(token != Token.eoft)
            {
                Console.WriteLine($"Error - Line {lineNumber} - Expected {expectedToken}, actual '{lexemes}'");
                System.Environment.Exit(0);
            }
        }

        public static void ThrowCustomException(string description)
        {
            Console.WriteLine($"Error - Line {lineNumber} - Expected {description}, actual '{lexemes}'");
            System.Environment.Exit(0);
        }
    }
}
