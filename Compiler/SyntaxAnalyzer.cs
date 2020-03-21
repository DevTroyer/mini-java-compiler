// Tucker Troyer
// Compiler Construction
// Assignment 3 - Syntax Analyzer
// Dr. Hamer
// 2/21/2020

namespace Compiler
{
    class SyntaxAnalyzer : Resources
    {
        LexicalAnalyzer lexicalAnalyzer { get; set; }
        SymbolTable symbolTable { get; set; }

        /// <summary>
        /// Constructor for the Syntax Analyzer class.
        /// </summary>
        /// <param name="commandLineFileName"></param>
        public SyntaxAnalyzer(string commandLineFileName)
        {
            lexicalAnalyzer = new LexicalAnalyzer(commandLineFileName);
            symbolTable = new SymbolTable();

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
            else if(token != Token.eoft)
            {
                ExceptionHandler.ThrowException(inputToken);
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
            depth++;
            Match(Token.publict);
            Match(Token.statict);
            Match(Token.voidt);
            Match(Token.maint);
            Match(Token.lparentt);
            depth++;
            Match(Token.Stringt);
            Match(Token.lbrackt);
            Match(Token.rbrackt);
            Match(Token.idt);
            Match(Token.rparentt);
            Match(Token.lcurlyt);
            SequenceOfStatements();
            Match(Token.rcurlyt);
            depth--;
            Match(Token.rcurlyt);
            depth--;
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
            else if(token != Token.finalt)
            {
                ExceptionHandler.ThrowException(Token.classt);
            }
        }

        /// <summary>
        /// The ClassDeclaration method for the Syntax Analyzer.
        /// </summary>
        private void ClassDeclaration()
        {
            Match(Token.classt);

            ISymbolTableEntry entry = symbolTable.CreateTableEntry(EntryType.classEntry);

            Match(Token.idt);
            if (token == Token.extendst)
            {
                Match(Token.extendst);
                Match(Token.idt);
            }
            Match(Token.lcurlyt);
            depth++;
            VariableDeclaration();
            MethodDeclaration();
            Match(Token.rcurlyt);

            symbolTable.ConvertEntryToClassEntry(entry);
            depth--;
        }

        /// <summary>
        /// The VariableDeclaration method for the Syntax Analyzer.
        /// </summary>
        private void VariableDeclaration()
        {
            if (token == Token.finalt)
            {
                Match(Token.finalt);
                if (Types.Contains(token))
                {
                    Type();
                    Match(Token.idt);
                    Match(Token.assignopt);
                    Match(Token.numt);
                    Match(Token.semit);
                    VariableDeclaration();
                }
                else
                {
                    ExceptionHandler.ThrowCustomException("return type");
                }
            }
            else if (Types.Contains(token))
            {
                Type();
                IdentifierList();
                Match(Token.semit);
                VariableDeclaration();
            }
            else if (token != Token.rcurlyt && token != Token.publict && token != Token.returnt)
            {
                ExceptionHandler.ThrowCustomException("variable or method declaration");
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
                    dataType = DataType.intType;
                    Match(Token.intt);
                    break;
                case Token.booleant:
                    dataType = DataType.booleanType;
                    Match(Token.booleant);
                    break;
                case Token.voidt:
                    dataType = DataType.voidType;
                    Match(Token.voidt);
                    break;
                case Token.floatt:
                    dataType = DataType.floatType;
                    Match(Token.floatt);
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
            else if(token == Token.idt)
            {
                ExceptionHandler.ThrowException(Token.commat);
            }
            else if (token != Token.semit)
            {
                ExceptionHandler.ThrowException(Token.semit);
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
                if (Types.Contains(token))
                {
                    Type();
                    Match(Token.idt);
                    Match(Token.lparentt);
                    depth++;
                    FormalList();
                    Match(Token.rparentt);
                    Match(Token.lcurlyt);
                    VariableDeclaration();
                    SequenceOfStatements();
                    Match(Token.returnt);
                    Expr();
                    Match(Token.semit);
                    Match(Token.rcurlyt);
                    depth--;
                    MethodDeclaration();
                }
                else
                {
                    ExceptionHandler.ThrowCustomException("return type");
                }
            }
            else if(token != Token.rcurlyt)
            {
                ExceptionHandler.ThrowException(Token.publict);
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
            if (Types.Contains(token))
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
                if (Types.Contains(token))
                {
                    Type();
                    Match(Token.idt);
                    FormalRest();
                }
            }
            else if(token != Token.rparentt)
            {
                ExceptionHandler.ThrowException(Token.commat);
            }
        }
    }
}

