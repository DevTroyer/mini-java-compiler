// Tucker Troyer
// Compiler Construction
// Assignment 1 - Lexical Analyzer
// Dr. Hamer
// 2/5/2020

using System;
using System.IO;

namespace Compiler
{
    public class LexicalAnalyzer : Resources
    {
        /// <summary>
        /// The constructor for the Lexical Analyzer class. Automatically sets token to unknown,
        /// verifies the existence of the command line file in the directory, and stores the
        /// reserved words into a reservedWords string array. If the command line file is found,
        /// the file is opened and the contents of the file are saved into a StreamReader as a bit-stream.
        /// </summary>
        /// <param name="commandLineFileName"></param>
        public LexicalAnalyzer(string commandLineFileName)
        {
            // Set token to unknown
            token = Token.unknownt;

            // Set the line number to 1
            lineNumber = 1;

            // Verify the existence of the command line file in the directory
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), commandLineFileName);
            if (File.Exists(commandLineFileName))
            {
                // Open command line file and save contents into streamReader as a bit-stream
                streamReader = File.OpenText(filePath);
            }
            else
            {
                Console.WriteLine("The specified file does not exist in the working directory.");
            }

            // Store reserved words into string array
            StoreReservedWords();
            
            // Get first character of the command line input file stream
            GetNextCharacter();
        }

        /// <summary>
        /// Method that stores the desired reserved words into the reservedWords string array.
        /// </summary>
        private void StoreReservedWords()
        {
            reservedWords.Add("final");
            reservedWords.Add("class");
            reservedWords.Add("public");
            reservedWords.Add("static");
            reservedWords.Add("void");
            reservedWords.Add("main");
            reservedWords.Add("String");
            reservedWords.Add("extends");
            reservedWords.Add("return");
            reservedWords.Add("int");
            reservedWords.Add("float");
            reservedWords.Add("boolean");
            reservedWords.Add("if");
            reservedWords.Add("else");
            reservedWords.Add("while");
            reservedWords.Add("System.out.println");
            reservedWords.Add("length");
            reservedWords.Add("true");
            reservedWords.Add("false");
            reservedWords.Add("this");
            reservedWords.Add("new");
        }

        /// <summary>
        /// Method that clears the lexemes.
        /// </summary>
        private void ClearLexemes()
        {
            lexemes.Clear();
        }

        /// <summary>
        /// Method that appends a character to the lexemes.
        /// </summary>
        private void AppendLexeme()
        {
            lexemes.Append(character);
        }

        /// <summary>
        /// Method that gets the next character.
        /// </summary>
        private void GetNextCharacter()
        {
            character = (char)streamReader.Read();
        }

        /// <summary>
        /// Method that gets the next Token.
        /// </summary>
        public void GetNextToken()
        {
            ClearLexemes();
            ClearValues();
            RemoveWhitespace();
            RemoveComment();
            EndOfFileCheck();
        }

        /// <summary>
        /// Clears the global values for 'value' and 'valueR'.
        /// </summary>
        private void ClearValues()
        {
            value = null;
            valueR = null;
            literal = null;
        }

        /// <summary>
        /// Method that eats up whitespace.
        /// </summary>
        private void RemoveWhitespace()
        {
            while (Char.IsWhiteSpace(character))
            {
                if(character == '\n')
                {
                    GetNextCharacter();
                    lineNumber++;
                }
                else
                {
                    GetNextCharacter();
                }
            }
        }

        /// <summary>
        /// Facilitates calls to the comment remover methods.
        /// </summary>
        private void RemoveComment()
        {
            if (character == '/' && streamReader.Peek() == '/')
            {
                AppendLexeme();
                GetNextCharacter();
                RemoveSingleLineComment();
            }
            else if (character == '/' && streamReader.Peek() == '*') 
            {
                AppendLexeme();
                GetNextCharacter();
                RemoveMultipleLineComment();
            }
        }

        /// <summary>
        /// Method that handles single-line comments.
        /// </summary>
        private void RemoveSingleLineComment()
        {
            ClearLexemes();
            while (character != '\n' && !streamReader.EndOfStream)
            {
                GetNextCharacter();
            }
            
            if(character == '\n')
            {
                lineNumber++;
                GetNextCharacter();
                RemoveWhitespace();
            }

            if(character == '/')
            {
                RemoveComment();
            }
        }

        /// <summary>
        /// Method that handles multiple-line comments.
        /// </summary>
        private void RemoveMultipleLineComment()
        {
            string multipleLineCommentEndTag = "*/";
            string concatString = string.Empty;
            while (!streamReader.EndOfStream && concatString != multipleLineCommentEndTag)
            {
                concatString = string.Empty;
                GetNextCharacter();
                if (character == '\n')
                {
                    lineNumber++;
                }
                concatString += character;
                concatString += (char)streamReader.Peek();
            }

            if(concatString == multipleLineCommentEndTag)
            {
                GetNextCharacter();
                GetNextCharacter();
                RemoveWhitespace();
                if (character == '/')
                {
                    RemoveComment();
                }
            }
            ClearLexemes();
        }

        /// <summary>
        /// Method that checks to see if the end of the file has been reached. If it has, it closes the file.
        /// </summary>
        private void EndOfFileCheck()
        {
            if (!streamReader.EndOfStream)
            {
                ProcessToken();
            }
            else
            {
                ProcessToken();
                ClearLexemes();
                token = Token.eoft;
            }
        }

        /// <summary>
        /// Method that processes a single token.
        /// </summary>
        private void ProcessToken()
        {
            AppendLexeme();
            GetNextCharacter();

            switch (lexemes[0].ToString())
            {
                case var match when wordTokenRegex.IsMatch(match):
                    ProcessWordToken();
                    break;
                case var match when numTokenRegex.IsMatch(match):
                    ProcessNumToken();
                    break;
                case "\"":
                    ProcessLiteralToken();
                    break;
                case var match when singleTokenRegex.IsMatch(match):
                    ProcessSingleToken();
                    break;
                case var match when doubleTokenRegex.IsMatch(match):
                    ProcessDoubleToken();
                    break;
                default:
                    ProcessSingleToken();
                    break;
            }
        }

        /// <summary>
        /// Method that processes a literal token.
        /// </summary>
        private void ProcessLiteralToken()
        {
            token = Token.quotet;
            ClearLexemes();
            while (character != '"' && character != '\n' && !streamReader.EndOfStream)
            {
                if(character == '\r')
                {
                    GetNextCharacter();
                }
                else
                {
                    AppendLexeme();
                    GetNextCharacter();
                }
            }
            if(character == '"')
            {
                token = Token.literalt;
                literal = lexemes.ToString();
                ClearLexemes();
                literal = null;
                AppendLexeme();
                token = Token.quotet;
                GetNextCharacter();
            }
            else if(character == '\n')
            {
                RemoveWhitespace();
                token = Token.literalt;
                literal = lexemes.ToString();
            }
            else
            {
                token = Token.literalt;
                GetNextCharacter();
            }
        }

        /// <summary>
        /// Method that processes a word token.
        /// </summary>
        private void ProcessWordToken()
        {
            while (identifiedWordTokenRegex.IsMatch(character.ToString()))
            {
                AppendLexeme();
                GetNextCharacter();
                if(lexemes.Equals("System"))
                {
                    AppendLexeme();
                    GetNextCharacter();
                }
                if (lexemes.Equals("System.out"))
                {
                    AppendLexeme();
                    GetNextCharacter();
                }
            }
            if(lexemes.ToString().Length > 31)
            {
                token = Token.unknownt;
            }
            else
            {
                token = IdentifyWordToken();
            }
        }

        /// <summary>
        /// This method identifies a token if it exists as a reserved word
        /// </summary>
        /// <returns></returns>
        private Token IdentifyWordToken()
        {
            switch (lexemes.ToString())
            {
                case "final":
                    token = Token.finalt;
                    break;
                case "class":
                    token = Token.classt;
                    break;
                case "public":
                    token = Token.publict;
                    break;
                case "static":
                    token = Token.statict;
                    break;
                case "void":
                    token = Token.voidt;
                    break;
                case "main":
                    token = Token.maint;
                    break;
                case "String":
                    token = Token.Stringt;
                    break;
                case "extends":
                    token = Token.extendst;
                    break;
                case "return":
                    token = Token.returnt;
                    break;
                case "int":
                    token = Token.intt;
                    break;
                case "float":
                    token = Token.floatt;
                    break;
                case "boolean":
                    token = Token.booleant;
                    break;
                case "if":
                    token = Token.ift;
                    break;
                case "else":
                    token = Token.elset;
                    break;
                case "while":
                    token = Token.whilet;
                    break;
                case "System.out.println":
                    token = Token.printlnt;
                    break;
                case "length":
                    token = Token.lengtht;
                    break;
                case "true":
                    token = Token.truet;
                    break;
                case "false":
                    token = Token.falset;
                    break;
                case "this":
                    token = Token.thist;
                    break;
                case "new":
                    token = Token.newt;
                    break;
                default:
                    token = Token.idt;
                    break;
            }
            return token;
        }

        /// <summary>
        /// Method that processes a single token.
        /// </summary>
        private void ProcessSingleToken()
        {
            if (lexemes.ToString() == ".") { token = Token.periodt; }
            else if (lexemes.ToString() == ";") { token = Token.semit; }
            else if (lexemes.ToString() == ",") { token = Token.commat; }
            else if (lexemes.ToString() == "(") { token = Token.lparentt; }
            else if (lexemes.ToString() == ")") { token = Token.rparentt; }
            else if (lexemes.ToString() == "[") { token = Token.lbrackt; }
            else if (lexemes.ToString() == "]") { token = Token.rbrackt; }
            else if (lexemes.ToString() == "{") { token = Token.lcurlyt; }
            else if (lexemes.ToString() == "}") { token = Token.rcurlyt; }
            else if (lexemes.ToString() == "/") { token = Token.mulopt; }
            else if (lexemes.ToString() == "*") { token = Token.mulopt; }
            else if (lexemes.ToString() == "+") { token = Token.addopt; }
            else if (lexemes.ToString() == "-") { token = Token.addopt; }
            else if (lexemes.ToString() == "!") { token = Token.negateopt; }
            else { token = Token.unknownt; }
        }

        /// <summary>
        /// Method that processes a double token.
        /// </summary>
        private void ProcessDoubleToken()
        {
            if (relationalOpDeciderRegex.IsMatch(lexemes[0].ToString()))
            {
                if(character != '=')
                {
                    if (lexemes.ToString() == ">") { token = Token.relopt; }
                    else if (lexemes.ToString() == "<") { token = Token.relopt; }
                    else if (lexemes.ToString() == "=") { token = Token.assignopt; }
                }
                else
                {
                    AppendLexeme();
                    if (lexemes.ToString() == "!=") { token = Token.relopt; }
                    else if (lexemes.ToString() == "<=") { token = Token.relopt; }
                    else if (lexemes.ToString() == ">=") { token = Token.relopt; }
                    else if (lexemes.ToString() == "==") { token = Token.relopt; }
                    GetNextCharacter();
                }
            }
            else if (inclusionOpDeciderRegex.IsMatch(lexemes[0].ToString()))
            {
                if (lexemes[0].ToString() == "&" && character == '&')
                {
                    AppendLexeme();
                    token = Token.mulopt;
                    GetNextCharacter();
                }
                else if(lexemes[0].ToString() == "|" && character == '|')
                {
                    AppendLexeme();
                    token = Token.addopt;
                    GetNextCharacter();
                }
                else
                {
                    token = Token.unknownt;
                }
            }
        }

        /// <summary>
        /// Method that processes a number token.
        /// </summary>
        private void ProcessNumToken()
        {
             while (numDeciderRegex.IsMatch(character.ToString()))
            {
                AppendLexeme();
                GetNextCharacter();
            }

            int lexLength = lexemes.Length;
            if (lexemes[lexLength - 1] == '.')
            {
                token = Token.unknownt;
            }
            else
            {
                token = Token.numt;
            }

            if (token != Token.unknownt)
            {
                if (lexemes.ToString().Contains('.'))
                {
                    valueR = Convert.ToDouble(lexemes.ToString());
                }
                else
                {
                    value = Convert.ToInt32(lexemes.ToString());
                }
            }
        }

        /// <summary>
        /// Method that displays a single token.
        /// </summary>
        public void DisplayToken()
        {
            Console.WriteLine("{0,-8}{1,-12}{2,-40}{3,-14}{4,-14}{5,-40}", lineNumber, token, lexemes, value, valueR, literal);
        }
    }
}
