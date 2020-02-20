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
            }
        }

        internal static void ThrowCustomException()
        {
            // Implement later
        }
    }
}
