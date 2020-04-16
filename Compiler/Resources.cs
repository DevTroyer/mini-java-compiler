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
        public static Regex singleTokenRegex = new Regex(@"[-+*!/(){},;.\[\]]");
        public static Regex doubleTokenRegex = new Regex(@"[=!<>|&]");
        public static Regex relationalOpDeciderRegex = new Regex("[<>!=]");
        public static Regex inclusionOpDeciderRegex = new Regex("[|&]");
        public static Regex numDeciderRegex = new Regex("[0-9.]");

        public static List<string> reservedWords = new List<string>(21)
        {
            "final", "class", "public", "static", "void", "main", "String", "extends", "return", "int", "float", "boolean",
            "if", "else", "while", "System.out.println", "length", "true", "false", "this", "new"
        };

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

        public static List<Token> FactorTokens = new List<Token>()
        {
            Token.idt, Token.numt, Token.lparentt, Token.negateopt, Token.addopt,
            Token.truet, Token.falset
        };

        public static List<string> referenceParameters = new List<string>();

        #endregion

        #region Symbol Table Resources

        // Global
        public static int depth;
        public static DataType dataType;
        public static int offset = 0;
        public static EntryType context;

        public enum DataType { voidType, booleanType, intType, None, floatType };
        public enum EntryType { tableEntry, classEntry, methodEntry, varEntry, constEntry };

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

        #region Intermediate Code Generator Resources

        public static int temporaryVariableCounter;
        public static string temporaryVariable;
        public static int temporaryVariableOffset;
        public static string code;
        public static Variable Tplace { get; set; }
        public static Variable Rplace { get; set; }
        public static Variable Eplace { get; set; }
        public static string BpOffsetNotation;
        public static int something;
        public static Stack<string> stack = new Stack<string>();
        public static Stack<string> tempVars = new Stack<string>();

        #endregion

        // Global enumerated date type that holds all of the tokens
        public enum Token
        {
            finalt, classt, publict, statict, voidt, maint, Stringt, extendst, returnt, intt, floatt, booleant,
            ift, elset, whilet, printlnt, lengtht, truet, falset, thist, newt, addopt, mulopt, assignopt,
            relopt, lparentt, rparentt, lcurlyt, rcurlyt, lbrackt, rbrackt, commat, literalt, semit,
            periodt, quotet, numt, idt, eoft, negateopt, unknownt
        };

        // Global variables
        public static StreamReader streamReader;
        public static int lineNumber;
        public static Token token;
        public static char character;
        public static string literal;
        public static int? value;
        public static double? valueR;
        public static StringBuilder lexeme = new StringBuilder(31);
    }
}
