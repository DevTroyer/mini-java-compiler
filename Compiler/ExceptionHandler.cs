using System;

namespace Compiler
{
    class ExceptionHandler : Resources
    {
        public static void ThrowException()
        {
            if(token != Token.eoft)
            {
                Console.WriteLine($"Error - Line {lineNumber} - Expected {1}, found '{lexemes}'");
            }
        }

        private void CalculateExpectedLexemes()
        {
            // Implement
        }

        internal static void ThrowCustomException()
        {
            // Implement
        }
    }
}
