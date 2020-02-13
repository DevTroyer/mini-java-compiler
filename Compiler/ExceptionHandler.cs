using System;

namespace Compiler
{
    class ExceptionHandler : Resources
    {
        public static void ThrowException(int lineNumber, Token inputToken)
        {
            if(inputToken != Token.eoft)
            {
                Console.WriteLine($"Error on line {0}, expected: actual: {1}", lineNumber, inputToken);
            }
        }
    }
}
