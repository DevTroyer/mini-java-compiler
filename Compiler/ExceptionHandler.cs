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
            if (token != Token.eoft)
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

        /// <summary>
        /// Method to throw a duplicate entry exception.
        /// </summary>
        /// <param name="lexeme"></param>
        public static void ThrowDuplicateEntryException(string lexeme)
        {
            Console.WriteLine($"Error - Line {lineNumber} - '{lexeme}' already exists in the current context");
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Method to throw an unused tokens exception.
        /// </summary>
        public static void ThrowUnusedTokensException()
        {
            Console.WriteLine($"Error - Line {lineNumber} - Unused tokens");
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Method to throw an undeclared lexemes exception.
        /// </summary>
        public static void ThrowUndeclaredLexemesException(string lexeme)
        {
            Console.WriteLine($"Error - Line {lineNumber} - Lexeme '{lexeme}' is undeclared");
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Method to throw an expected valid exception exception.
        /// </summary>
        public static void ThrowExpectedValidExpressionException(string lexeme)
        {
            Console.WriteLine($"Error - Line {lineNumber} - Expected valid expression, instead found {lexeme}");
            System.Environment.Exit(0);
        }
    }
}
