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
                ExceptionHandler.ThrowUnexpectedTokenException(inputToken);
            }
        }

        /// <summary>
        /// The Prog method for the Syntax Analyzer.
        /// </summary>
        public void Prog()
        {
            MoreClasses();
            MainClass();

            // Display the lexemes at depth 0 - for testing purposes
            symbolTable.WriteTable(0);
        }

        /// <summary>
        /// The MainClass method for the Syntax Analyzer.
        /// </summary>
        private void MainClass()
        {
            Match(Token.finalt);
            Match(Token.classt);

            ISymbolTableEntry entry = symbolTable.CreateTableEntry(EntryType.tableEntry);

            Match(Token.idt);
            Match(Token.lcurlyt);
            depth++;
            Match(Token.publict);
            Match(Token.statict);
            Match(Token.voidt);

            context = EntryType.methodEntry;
            ISymbolTableEntry mainEntry = symbolTable.CreateTableEntry(EntryType.tableEntry);
            listOfMethodNames.Add(lexemes.ToString());
            returnType = DataType.voidType;

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
            symbolTable.WriteTable(depth);
            symbolTable.DeleteDepth(depth);
            depth--;
            Match(Token.rcurlyt);

            symbolTable.ConvertEntryToClassEntry(entry);
            sizeOfLocalVariables = 0;
            listOfVariableNames.Clear();
            listOfMethodNames.Clear();
            symbolTable.WriteTable(depth);
            symbolTable.DeleteDepth(depth);
            depth--;
        }

        #region Sequence of statements

        private void SequenceOfStatements()
        {
            if (token == Token.idt)
            {
                Statement();
                Match(Token.semit);
                SequenceOfStatements();
            }
        }

        private void Statement()
        {
            AssignStat();
            IOStat();
        }

        private void AssignStat()
        {
            ISymbolTableEntry entry = symbolTable.Lookup(lexemes.ToString());

            if (entry != null)
            {
                Match(Token.idt);
                Match(Token.assignopt);
                Expr();
            }
            else
            {
                ExceptionHandler.ThrowUndeclaredIdentifierException(lexemes.ToString());
            } 
        }

        private void IOStat()
        {
            // Implementation pending
        }

        private void Expr()
        {
            if (FactorTokens.Contains(token))
            {
                Relation();
            }
        }

        private void Relation()
        {
            SimpleExpr();
        }

        private void SimpleExpr()
        {
            Term();
            MoreTerm();
        }

        private void Term()
        {
            Factor();
            MoreFactor();
        }

        private void Factor()
        {
            if (token == Token.idt)
            {
                Match(Token.idt);
            }
            else if (token == Token.numt)
            {
                Match(Token.numt);
            }
            else if (token == Token.lparentt)
            {
                Match(Token.lparentt);
                Expr();
                Match(Token.rparentt);
            }
            else if (token == Token.negateopt)
            {
                Match(Token.negateopt);
                Factor();
            }
            else if (token == Token.addopt && lexemes.ToString() == "-")
            {
                SignOp();
                Factor();
            }
            else if (token == Token.truet)
            {
                Match(Token.truet);
            }
            else if (token == Token.falset)
            {
                Match(Token.falset);
            }
            else
            {
                ExceptionHandler.ThrowInvalidExpressionException(lexemes.ToString());
            }
        }

        private void MoreFactor()
        {
            if (token == Token.mulopt)
            {
                MulOp();
                Factor();
                MoreFactor();
            }
        }

        private void MoreTerm()
        {
            if (token == Token.addopt)
            {
                AddOp();
                Term();
                MoreTerm();
            }
        }

        private void AddOp()
        {
            Match(Token.addopt);
        }

        private void MulOp()
        {
            Match(Token.mulopt);
        }

        private void SignOp()
        {
            Match(Token.addopt);
        }

        #endregion

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
                ExceptionHandler.ThrowUnexpectedTokenException(Token.classt);
            }
        }

        /// <summary>
        /// The ClassDeclaration method for the Syntax Analyzer.
        /// </summary>
        private void ClassDeclaration()
        {
            Match(Token.classt);

            ISymbolTableEntry entry = symbolTable.CreateTableEntry(EntryType.tableEntry);
            context = EntryType.classEntry;

            Match(Token.idt);
            if (token == Token.extendst)
            {
                Match(Token.extendst);
                Match(Token.idt);
            }
            Match(Token.lcurlyt);
            depth++;
            VariableDeclaration();
            offset = 0;
            MethodDeclaration();
            Match(Token.rcurlyt);

            symbolTable.ConvertEntryToClassEntry(entry);
            sizeOfLocalVariables = 0;
            listOfVariableNames.Clear();
            listOfMethodNames.Clear();
            symbolTable.WriteTable(depth);
            symbolTable.DeleteDepth(depth);
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

                    ISymbolTableEntry entry = symbolTable.CreateTableEntry(EntryType.tableEntry);
                    if(context == EntryType.classEntry)
                    {
                        listOfVariableNames.Add(lexemes.ToString());
                        sizeOfLocalVariables += size;
                    }
                    else
                    {
                        sizeOfLocalMethodVariables += size;
                    }

                    Match(Token.idt);
                    Match(Token.assignopt);
                    if(value != null)
                    {
                        symbolTable.ConvertEntryToConstIntEntry(entry);
                    }
                    else if(valueR != null)
                    {
                        symbolTable.ConvertEntryToConstDoubleEntry(entry);
                    }
                    offset += (int)dataType;

                    Match(Token.numt);
                    Match(Token.semit);
                    VariableDeclaration();
                }
                else
                {
                    ExceptionHandler.ThrowCustomMessageException("return type");
                }
            }
            else if (Types.Contains(token))
            {
                Type();
                IdentifierList();
                Match(Token.semit);
                VariableDeclaration();
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
                    size = 2;
                    Match(Token.intt);
                    break;
                case Token.booleant:
                    dataType = DataType.booleanType;
                    size = 1;
                    Match(Token.booleant);
                    break;
                case Token.voidt:
                    dataType = DataType.voidType;
                    size = 0;
                    Match(Token.voidt);
                    break;
                case Token.floatt:
                    dataType = DataType.floatType;
                    size = 4;
                    Match(Token.floatt);
                    break;
            }
        }

        /// <summary>
        /// The IdentifierList method for the Syntax Analyzer.
        /// </summary>
        private void IdentifierList()
        {
            ISymbolTableEntry entry = symbolTable.CreateTableEntry(EntryType.tableEntry);
            if (context == EntryType.classEntry)
            {
                listOfVariableNames.Add(lexemes.ToString());
                sizeOfLocalVariables += size;
            }
            else
            {
                sizeOfLocalMethodVariables += size;
            }
            symbolTable.ConvertEntryToVarEntry(entry);
            offset += (int)dataType;

            Match(Token.idt);
            
            if(token == Token.commat)
            {
                Match(Token.commat);
                IdentifierList();
            }
            else if(token == Token.idt)
            {
                ExceptionHandler.ThrowUnexpectedTokenException(Token.commat);
            }
            else if (token != Token.semit)
            {
                ExceptionHandler.ThrowUnexpectedTokenException(Token.semit);
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

                    context = EntryType.methodEntry;
                    ISymbolTableEntry entry = symbolTable.CreateTableEntry(EntryType.tableEntry);
                    listOfMethodNames.Add(lexemes.ToString());
                    returnType = dataType;

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

                    symbolTable.ConvertEntryToMethodEntry(entry);
                    parameterType.Clear();
                    numOfParameters = 0;
                    sizeOfLocalMethodVariables = 0;
                    sizeOfFormalParameters = 0;

                    symbolTable.WriteTable(depth);
                    symbolTable.DeleteDepth(depth);
                    depth--;
                    MethodDeclaration();
                }
                else
                {
                    ExceptionHandler.ThrowCustomMessageException("return type");
                }
            }
            else if(token != Token.rcurlyt)
            {
                ExceptionHandler.ThrowUnexpectedTokenException(Token.publict);
            }
        }

        /// <summary>
        /// The FormalList method for the Syntax Analyzer.
        /// </summary>
        private void FormalList()
        {
            if (Types.Contains(token))
            {
                Type();

                offset = 0;
                ISymbolTableEntry entry = symbolTable.CreateTableEntry(EntryType.tableEntry);
                sizeOfFormalParameters += size;
                numOfParameters++;
                parameterType.Add(dataType);

                Match(Token.idt);

                symbolTable.ConvertEntryToVarEntry(entry);

                FormalRest();
                offset += sizeOfFormalParameters;
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

                    offset = sizeOfFormalParameters;
                    ISymbolTableEntry entry = symbolTable.CreateTableEntry(EntryType.tableEntry);
                    sizeOfFormalParameters += size;
                    numOfParameters++;
                    parameterType.Add(dataType);

                    Match(Token.idt);

                    symbolTable.ConvertEntryToVarEntry(entry);
                    FormalRest();
                }
            }
            else if(token != Token.rparentt)
            {
                ExceptionHandler.ThrowUnexpectedTokenException(Token.commat);
            }
        }
    }
}

