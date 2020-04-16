using System.IO;

namespace Compiler
{
    class SyntaxAnalyzer : Resources
    {
        LexicalAnalyzer lexicalAnalyzer { get; set; }
        SymbolTable symbolTable { get; set; }
        IntermediateCodeGenerator intermediateCodeGenerator { get; set; }

        /// <summary>
        /// Constructor for the Syntax Analyzer class.
        /// </summary>
        /// <param name="commandLineFileName"></param>
        public SyntaxAnalyzer(string commandLineFileName)
        {
            lexicalAnalyzer = new LexicalAnalyzer(commandLineFileName);
            symbolTable = new SymbolTable();
            intermediateCodeGenerator = new IntermediateCodeGenerator();

            intermediateCodeGenerator.SetupTacFile(commandLineFileName);

            // Prime the parser
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

            //// Display the lexemes at depth 0 - for testing purposes
            //symbolTable.WriteTable(0);
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
            intermediateCodeGenerator.Emit($"proc {mainEntry.Lexeme}");
            listOfMethodNames.Add(lexeme.ToString());
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
            intermediateCodeGenerator.Emit($"endp {mainEntry.Lexeme}");
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
            ISymbolTableEntry entry = symbolTable.Lookup(lexeme.ToString());

            if (entry != null && entry.TypeOfEntry == EntryType.classEntry)
            {
                MethodCall();
            }
            else if (entry != null && entry.TypeOfEntry == EntryType.varEntry)
            {
                Match(Token.idt);
                Match(Token.assignopt);

                ISymbolTableEntry entry2 = symbolTable.Lookup(lexeme.ToString());
                if (entry2 != null && entry2.TypeOfEntry == EntryType.classEntry)
                {
                    MethodCall();
                    intermediateCodeGenerator.Emit($"{entry.Lexeme} = _AX");
                }
                else
                {
                    Expr();
                    intermediateCodeGenerator.GenerateFinalExpression(entry as Variable);
                    intermediateCodeGenerator.Emit(code);
                }
            }
            else
            {
                // undeclared lexeme
            }
        }

        private void MethodCall()
        {
            Match(Token.idt);
            Match(Token.periodt);
            ISymbolTableEntry entry = symbolTable.Lookup(lexeme.ToString());
            if (entry != null)
            {
                if(entry.TypeOfEntry == EntryType.methodEntry)
                {
                    Match(Token.idt);
                    Match(Token.lparentt);
                    Params();
                    if(referenceParameters.Count > 0)
                    {
                        for (int i = referenceParameters.Count - 1; i >= 0; i--)
                        {
                            ISymbolTableEntry tableEntry = symbolTable.Lookup(referenceParameters[i]);
                            Variable parameterEntry = tableEntry as Variable;
                            intermediateCodeGenerator.Emit($"push {parameterEntry.OffsetNotation}");
                        }
                        referenceParameters.Clear();
                    }
                    intermediateCodeGenerator.Emit($"call {entry.Lexeme}");
                    Match(Token.rparentt);
                }
                else
                {
                    ExceptionHandler.ThrowUndeclaredIdentifierException(lexeme.ToString());
                }
            }
            else
            {
                ExceptionHandler.ThrowUndeclaredIdentifierException(lexeme.ToString());
            }
        }

        private void Params()
        {
            if(token != Token.rparentt)
            {
                if (token == Token.idt)
                {
                    referenceParameters.Add(lexeme.ToString());
                    Match(Token.idt);
                    ParamsTail();
                }
                else if (token == Token.numt)
                {
                    referenceParameters.Add(lexeme.ToString());
                    Match(Token.numt);
                    ParamsTail();
                }
                else
                {
                    // Throw exception
                }
            }
        }

        private void ParamsTail()
        {
            if(token == Token.commat)
            {
                Match(Token.commat);
                if (token == Token.idt)
                {
                    referenceParameters.Add(lexeme.ToString());
                    Match(Token.idt);
                    ParamsTail();
                }
                else if (token == Token.numt)
                {
                    referenceParameters.Add(lexeme.ToString());
                    Match(Token.numt);
                    ParamsTail();
                }
                else
                {
                    // Throw exception
                }
            }
        }

        private void IOStat()
        {
            // Implementation pending
        }

        private void Expr()
        {
            if(FactorTokens.Contains(token))
            {
                Relation();
                Eplace = Tplace;
            }
        }

        private void Relation()
        {
            SimpleExpr();
        }

        private void SimpleExpr()
        {
            Term();
            Rplace = Tplace;
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
                Tplace = symbolTable.Lookup(lexeme.ToString()) as Variable;
                
                if(Tplace != null)
                {
                    Match(Token.idt);
                }
                else
                {
                    // lexeme is undeclared
                }
            }
            else if (token == Token.numt)
            {
                Tplace = new Variable();
                temporaryVariable = lexeme.ToString();
                Tplace.Lexeme = lexeme.ToString();
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
            else if (token == Token.addopt && lexeme.ToString() == "-")
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
                ExceptionHandler.ThrowInvalidExpressionException(lexeme.ToString());
            }
        }

        private void MoreFactor()
        {
            if (token == Token.mulopt)
            {
                intermediateCodeGenerator.CreateTempVariable();
                intermediateCodeGenerator.GenerateThreeAddressCodeSegment();
                tempVars.Push(temporaryVariable);
                MulOp();
                stack.Push(code);
                Factor();
                code = stack.Pop();
                code += Tplace.OffsetNotation;
                Rplace.OffsetNotation = temporaryVariable;
                intermediateCodeGenerator.Emit(code);
                MoreFactor();
            }
        }

        private void MoreTerm()
        {
            if (token == Token.addopt)
            {
                intermediateCodeGenerator.CreateTempVariable();
                intermediateCodeGenerator.GenerateThreeAddressCodeSegment();
                tempVars.Push(temporaryVariable);
                AddOp();
                stack.Push(code);
                Term();
                code = stack.Pop();
                code += Tplace.OffsetNotation;
                Rplace.OffsetNotation = temporaryVariable;
                intermediateCodeGenerator.Emit(code);
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
            //symbolTable.WriteTable(depth);
            //symbolTable.DeleteDepth(depth);
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
                        listOfVariableNames.Add(lexeme.ToString());
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
                        symbolTable.ConvertEntryToIntConstantEntry(entry);
                    }
                    else if(valueR != null)
                    {
                        symbolTable.ConvertEntryToDoubleConstantEntry(entry);
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
                listOfVariableNames.Add(lexeme.ToString());
                sizeOfLocalVariables += size;
            }
            else
            {
                sizeOfLocalMethodVariables += size;
            }
            BpOffsetNotation = $"_BP-{offset}";
            symbolTable.ConvertEntryToVariableEntry(intermediateCodeGenerator, entry, false);
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
                    intermediateCodeGenerator.Emit($"proc {entry.Lexeme}");
                    listOfMethodNames.Add(lexeme.ToString());
                    returnType = dataType;

                    Match(Token.idt);
                    Match(Token.lparentt);
                    depth++;
                    sizeOfFormalParameters = 4;
                    FormalList();
                    Match(Token.rparentt);
                    Match(Token.lcurlyt);
                    offset = 2; //TODO
                    VariableDeclaration();
                    temporaryVariableCounter = offset - 2;
                    SequenceOfStatements();
                    temporaryVariableCounter = 0;
                    Match(Token.returnt);
                    Expr();
                    Match(Token.semit);
                    Match(Token.rcurlyt);

                    symbolTable.ConvertEntryToMethodEntry(entry);
                    intermediateCodeGenerator.Emit($"endp {entry.Lexeme}");
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
                BpOffsetNotation = $"_BP+{sizeOfFormalParameters}";

                sizeOfFormalParameters += size;
                numOfParameters++;
                parameterType.Add(dataType);

                Match(Token.idt);

                symbolTable.ConvertEntryToVariableEntry(intermediateCodeGenerator, entry, true);

                FormalRest();
                offset += (int)dataType;
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

                    ISymbolTableEntry entry = symbolTable.CreateTableEntry(EntryType.tableEntry);
                    BpOffsetNotation = $"_BP+{sizeOfFormalParameters}";

                    sizeOfFormalParameters += size;
                    numOfParameters++;
                    parameterType.Add(dataType);

                    Match(Token.idt);

                    symbolTable.ConvertEntryToVariableEntry(intermediateCodeGenerator, entry, true);
                    FormalRest();
                    offset += (int)dataType;
                }
            }
            else if(token != Token.rparentt)
            {
                ExceptionHandler.ThrowUnexpectedTokenException(Token.commat);
            }
        }
    }
}

