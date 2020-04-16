using System;

namespace Compiler
{
    class ExceptionHandler : Resources
    {
        /// <summary>
        /// Throws an exception for an unexpected token.
        /// </summary>
        /// <param name="expectedToken"></param>
        public static void ThrowUnexpectedTokenException(Token expectedToken)
        {
            if (token != Token.eoft)
            {
                Console.WriteLine($"Error ({lineNumber}): Expected {expectedToken}, actual '{lexeme}'");
                System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// Throws an exception with a custom message.
        /// </summary>
        /// <param name="description"></param>
        public static void ThrowCustomMessageException(string message)
        {
            Console.WriteLine($"Error ({lineNumber}): Expected {message}, actual '{lexeme}'");
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Thows an exception for a duplicate identifier (lexeme).
        /// </summary>
        /// <param name="lexeme"></param>
        public static void ThrowDuplicateIdentifierException(string lexeme)
        {
            Console.WriteLine($"Error ({lineNumber}): Identifier '{lexeme}' already exists in the current context");
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Throws an exception for unused tokens.
        /// </summary>
        public static void ThrowUnusedTokensException()
        {
            Console.WriteLine($"Error ({lineNumber}): Unused tokens encountered");
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Throws an exception for an undeclared identifier.
        /// </summary>
        public static void ThrowUndeclaredIdentifierException(string lexeme)
        {
            Console.WriteLine($"Error ({lineNumber}): Identifier '{lexeme}' is undeclared");
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Throw an exception for an invalid expression.
        /// </summary>
        public static void ThrowInvalidExpressionException(string lexeme)
        {
            Console.WriteLine($"Error ({lineNumber}): Expected a valid expression, instead found '{lexeme}'");
            System.Environment.Exit(0);
        }

        public static void ThrowFileExistsException(string filename)
        {
            Console.WriteLine($"Error: File with filename '{filename}' already exists in project directory");
            System.Environment.Exit(0);
        }

        public static void ThrowVariableOverflowException()
        {
            Console.WriteLine($"Error: Variable overflow of temporary variables during compilation");
            System.Environment.Exit(0);
        }
    }
}
