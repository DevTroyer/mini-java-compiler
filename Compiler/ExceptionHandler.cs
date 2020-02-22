using System;

namespace Compiler
{
    class ExceptionHandler : Resources
    {
        /// <summary>
        /// Method to throw a general exception message.
        /// </summary>
        /// <param name="expectedToken"></param>
        public static void ThrowException(Token expectedToken)
        {
            if(token != Token.eoft)
            {
                Console.WriteLine($"Error - Line {lineNumber} - Expected {expectedToken}, actual '{lexemes}'");
                System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// Method to throw a custom exception message.
        /// </summary>
        /// <param name="description"></param>
        public static void ThrowCustomException(string description)
        {
            Console.WriteLine($"Error - Line {lineNumber} - Expected {description}, actual '{lexemes}'");
            System.Environment.Exit(0);
        }
    }
}
