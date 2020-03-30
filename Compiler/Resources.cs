using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Compiler
{
    public class Resources
    {
        #region Lexical Analyzer Resources

        // Global regular expressions
        public static Regex wordTokenRegex = new Regex("[A-Za-z]");
        public static Regex identifiedWordTokenRegex = new Regex("[0-9A-Za-z_]");
        public static Regex numTokenRegex = new Regex("[0-9]");
        public static Regex singleTokenRegex = new Regex(@"[-+*/(){},;.\[\]]");
        public static Regex doubleTokenRegex = new Regex(@"[=!<>|&]");
        public static Regex relationalOpDeciderRegex = new Regex("[<>!=]");
        public static Regex inclusionOpDeciderRegex = new Regex("[|&]");
        public static Regex numDeciderRegex = new Regex("[0-9.]");

        #endregion

        #region Syntax Analyzer Resources

        // Return types
        public static List<Token> Types = new List<Token>(4)
        {
            Token.intt,
            Token.booleant,
            Token.voidt,
            Token.floatt
        };

        #endregion

        #region Symbol Table Resources

        // Global
        public static int depth;
        public static DataType dataType;
        public static int offset = 0;
        public static EntryType context;

        public enum DataType { voidType, booleanType, intType, None, floatType };
        public enum EntryType { classEntry, methodEntry, varEntry, constEntry };

        // Class-specific
        public static int sizeOfLocalVariables;
        public static List<string> listOfVariableNames = new List<string>();
        public static List<string> listOfMethodNames = new List<string>();

        // Variable-specific
        public static int size;

        // Method-specific
        public static int sizeOfLocalMethodVariables;
        public static int sizeOfFormalParameters;
        public static int numOfParameters;
        public static List<DataType> parameterType = new List<DataType>();
        public static DataType returnType;

        #endregion

        // Global enumerated date type that holds all of the tokens
        public enum Token
        {
            finalt, classt, publict, statict, voidt, maint, Stringt, extendst, returnt, intt, floatt, booleant,
            ift, elset, whilet, printlnt, lengtht, truet, falset, thist, newt, addopt, mulopt, assignopt,
            relopt, lparentt, rparentt, lcurlyt, rcurlyt, lbrackt, rbrackt, commat, literalt, semit,
            periodt, quotet, numt, idt, eoft, unknownt
        };

        // Global variables
        public static List<string> reservedWords = new List<string>(20);
        public static StreamReader streamReader;
        public static int lineNumber;
        public static Token token;
        public static char character;
        public static string literal;
        public static int? value;
        public static double? valueR;
        public static StringBuilder lexemes = new StringBuilder(31);
    }
}
