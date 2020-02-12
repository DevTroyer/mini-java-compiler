using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class SyntaxAnalyzer : Resources
    {
        LexicalAnalyzer lexicalAnalyzer { get; set; }

        /// <summary>
        /// Constructor for the Syntax Analyzer class.
        /// </summary>
        /// <param name="commandLineFileName"></param>
        public SyntaxAnalyzer(string commandLineFileName)
        {
            lexicalAnalyzer = new LexicalAnalyzer(commandLineFileName);

            // Prime the parser :O
            lexicalAnalyzer.GetNextToken();
        }

        /// <summary>
        /// The Match method for the Syntax Analyzer.
        /// </summary>
        /// <param name="tokenArgument"></param>
        private void Match(Token tokenArgument)
        {
            if (token == tokenArgument)
            {
                lexicalAnalyzer.GetNextToken();
            }
            else
            {
                Console.WriteLine("Error!");
            }
        }

        /// <summary>
        /// The Prog method for the Syntax Analyzer.
        /// </summary>
        public void Prog()
        {
            MainClass();
            MoreClasses();
        }

        private void MainClass()
        {
            Match(Token.finalt);
            Match(Token.classt);
            Match(Token.idt);
            Match(Token.lcurlyt);
            Match(Token.publict);
            Match(Token.statict);
            Match(Token.voidt);
            Match(Token.idt);
            Match(Token.lparentt);
            Match(Token.Stringt);
            Match(Token.lbrackt);
            Match(Token.rbrackt);
            Match(Token.idt);
            Match(Token.rparentt);
            Match(Token.lcurlyt);
            SequenceOfStatements();
            Match(Token.rcurlyt);
            Match(Token.rcurlyt);
        }

        private void SequenceOfStatements()
        {
            // Implementation pending
        }

        private void MoreClasses()
        {
            if (token == Token.classt)
            {
                ClassDeclaration();
                MoreClasses();
            }
        }

        private void ClassDeclaration()
        {
            Match(Token.classt);
            Match(Token.idt);
            if (token == Token.extendst)
            {
                Match(Token.extendst);
                Match(Token.idt);
            }
            Match(Token.lcurlyt);
            VariableDeclaration();
            MethodDeclaration();
            Match(Token.rcurlyt);
        }

        private void VariableDeclaration()
        {
            if (token == Token.finalt)
            {
                Match(Token.finalt);
            }
            if (token == Token.intt || token == Token.booleant || token == Token.voidt)
            {
                Type();
                IdentifierList();
                if (token == Token.assignopt)
                {
                    Match(Token.assignopt);
                    Match(Token.)
                }
                Match(Token.semit);
                VariableDeclaration();
            }
            else
            {
                // Thorw an error if not the right type
            }
        }

        private void Type()
        {
            if (token == Token.intt)
            {
                Match(Token.intt);
            }
            else if (token == Token.booleant)
            {
                Match(Token.booleant);
            }
            else if (token == Token.voidt)
            {
                Match(Token.voidt);
            }
        }
        private void MethodDeclaration()
        {

        }
    }
}

