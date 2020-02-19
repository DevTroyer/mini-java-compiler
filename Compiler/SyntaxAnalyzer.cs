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
        private void Match(Token inputToken)
        {
            if (token == inputToken)
            {
                lexicalAnalyzer.GetNextToken();
            }
            else
            {
                ExceptionHandler.ThrowException();
            }
        }

        /// <summary>
        /// The Prog method for the Syntax Analyzer.
        /// </summary>
        public void Prog()
        {
            MoreClasses();
            MainClass();
        }

        /// <summary>
        /// The MainClass method for the Syntax Analyzer.
        /// </summary>
        private void MainClass()
        {
            Match(Token.finalt);
            Match(Token.classt);
            Match(Token.idt);
            Match(Token.lcurlyt);
            Match(Token.publict);
            Match(Token.statict);
            Match(Token.voidt);
            Match(Token.maint);
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

        /// <summary>
        /// The SequenceOfStatements method for the Syntax Analyzer.
        /// </summary>
        private void SequenceOfStatements()
        {
            // Implementation pending
        }

        /// <summary>
        /// The MoreClasses method for the Syntax Analyzer.
        /// </summary>
        private void MoreClasses()
        {
            if (token == Token.classt)
            {
                ClassDeclaration();
                MoreClasses();
            }
            else if(token == Token.idt)
            {
                ExceptionHandler.ThrowCustomException();
            }
        }

        /// <summary>
        /// The ClassDeclaration method for the Syntax Analyzer.
        /// </summary>
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

        /// <summary>
        /// The VariableDeclaration method for the Syntax Analyzer.
        /// </summary>
        private void VariableDeclaration()
        {
            if (token == Token.finalt)
            {
                Match(Token.finalt);
            }
            if (returnTypes.Contains(token))
            {
                Type();
                IdentifierList();

                switch(token)
                {
                    case Token.assignopt:
                        Match(Token.assignopt);
                        Match(Token.numt);
                        Match(Token.semit);
                        VariableDeclaration();
                        break;
                    case Token.semit:
                        Match(Token.semit);
                        VariableDeclaration();
                        break;
                }
            }
            else
            {
                ExceptionHandler.ThrowCustomException();
            }
        }

        /// <summary>
        /// The Type method for the Syntax Analyzer.
        /// </summary>
        private void Type()
        {
            switch (token)
            {
                case Token.intt:
                    Match(Token.intt);
                    break;
                case Token.booleant:
                    Match(Token.booleant);
                    break;
                case Token.voidt:
                    Match(Token.voidt);
                    break;
            }
        }

        /// <summary>
        /// The IdentifierList method for the Syntax Analyzer.
        /// </summary>
        private void IdentifierList()
        {
            Match(Token.idt);
            if(token == Token.commat)
            {
                Match(Token.commat);
                IdentifierList();
            }
        }

        /// <summary>
        /// The MethodDeclaration method for the Syntax Analyzer.
        /// </summary>
        private void MethodDeclaration()
        {
            if(token == Token.publict)
            {
                Match(Token.publict);
                if (returnTypes.Contains(token))
                {
                    Type();
                    Match(Token.idt);
                    Match(Token.lparentt);
                    FormalList();
                    Match(Token.rparentt);
                    Match(Token.lcurlyt);
                    VariableDeclaration();
                    SequenceOfStatements();
                    Match(Token.returnt);
                    Expr();
                    Match(Token.semit);
                    Match(Token.rcurlyt);
                    MethodDeclaration();
                }
            }
        }

        /// <summary>
        /// The Expr method for the Syntax Analyzer.
        /// </summary>
        private void Expr()
        {
            // Implementation pending
        }

        /// <summary>
        /// The FormalList method for the Syntax Analyzer.
        /// </summary>
        private void FormalList()
        {
            if (returnTypes.Contains(token))
            {
                Type();
                Match(Token.idt);
                FormalRest();
            }
        }

        /// <summary>
        /// The FormalRest method for the Syntax Analyzer.
        /// </summary>
        private void FormalRest()
        {
            if (token == Token.commat)
            {
                Match(Token.commat);
                if (returnTypes.Contains(token))
                {
                    Type();
                    Match(Token.idt);
                    FormalRest();
                }
            }
        }
    }
}

