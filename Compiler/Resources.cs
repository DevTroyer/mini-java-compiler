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

        public static int depth;

        public enum VarType { booleanType, floatType, intType, voidType };
        public enum EntryType { classEntry, methodEntry, varEntry, constEntry };
        public enum ConstType { intType, floatType }
        public enum ParameterPassingMode { passByValue, passByReference };

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
        public static List<string> reservedWords = new List<string>(19);
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
