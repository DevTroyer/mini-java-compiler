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
            ISymbolTableEntry Eplace = null;
            string code = "";

            if (entry != null && (entry.TypeOfEntry == EntryType.classEntry || entry.TypeOfEntry == EntryType.tableEntry))
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
                    Expr(ref Eplace);
                    intermediateCodeGenerator.GenerateFinalExpression(entry, Eplace, ref code);
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

        private void Expr(ref ISymbolTableEntry Eplace)
        {
            ISymbolTableEntry Tplace = null;

            if(FactorTokens.Contains(token))
            {
                Relation(ref Tplace);
                Eplace = Tplace;
            }
        }

        private void Relation(ref ISymbolTableEntry Tplace)
        {
            SimpleExpr(ref Tplace);
        }

        private void SimpleExpr(ref ISymbolTableEntry Tplace)
        {
            Term(ref Tplace);
            //Rplace = Tplace;
            MoreTerm(ref Tplace);
        }

        private void Term(ref ISymbolTableEntry Tplace)
        {
            Factor(ref Tplace);
            MoreFactor(ref Tplace);
        }

        private void Factor(ref ISymbolTableEntry Tplace)
        {
            if (token == Token.idt)
            {
                //Tplace = symbolTable.Lookup(lexeme.ToString()) as Variable;
                ISymbolTableEntry Xplace = symbolTable.Lookup(lexeme.ToString());

                if(Xplace.TypeOfEntry == EntryType.constEntry)
                {
                    Tplace = Xplace as Constant;
                }
                else
                {
                    Tplace = Xplace as Variable;
                }                                               

                if (Tplace != null)
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
                Tplace.OffsetNotation = lexeme.ToString();
                intermediateCodeGenerator.GenerateTempExpressionTAC(ref Tplace);
                //Tplace.Lexeme = lexeme.ToString();
                Match(Token.numt);
            }
            else if (token == Token.lparentt)
            {
                Match(Token.lparentt);
                Expr(ref Tplace);
                Match(Token.rparentt);
            }
            else if (token == Token.negateopt)
            {
                Match(Token.negateopt);
                Factor(ref Tplace);
            }
            else if (token == Token.addopt && lexeme.ToString() == "-")
            {
                string code = "";
                string tempVarName = "";

                Tplace = new Variable();
                Tplace.TypeOfEntry = EntryType.varEntry;
                Tplace.OffsetNotation = "0";

                
                intermediateCodeGenerator.CreateTempVariable(ref tempVarName, Tplace);
                intermediateCodeGenerator.GenerateThreeAddressCodeSegment(ref code, tempVarName, Tplace);
                SignOp();
                Factor(ref Tplace);
                code += Tplace.OffsetNotation;
                Tplace.OffsetNotation = tempVarName;
                intermediateCodeGenerator.Emit(ref code);
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

        private void MoreFactor(ref ISymbolTableEntry Rplace)
        {
            string code = "";
            string tempVarName = "";
            ISymbolTableEntry Tplace = null;

            if (token == Token.mulopt)
            {
                intermediateCodeGenerator.CreateTempVariable(ref tempVarName, Rplace);
                intermediateCodeGenerator.GenerateThreeAddressCodeSegment(ref code, tempVarName, Rplace);
                //tempVars.Push(temporaryVariable);
                MulOp();
                //stack.Push(code);
                Factor(ref Tplace);
                //code = stack.Pop();
                code += Tplace.OffsetNotation;
                string value = Rplace.OffsetNotation;
                if (tempVarName != Rplace.OffsetNotation)
                {
                    Rplace.OffsetNotation = tempVarName;
                }
                else
                {
                    Rplace.OffsetNotation = value;
                }
                intermediateCodeGenerator.Emit(ref code);
                MoreFactor(ref Rplace);
            }
        }

        private void MoreTerm(ref ISymbolTableEntry Rplace)
        {
            string code = "";
            string tempVarName = "";
            ISymbolTableEntry Tplace = null;

            if (token == Token.addopt)
            {
                intermediateCodeGenerator.CreateTempVariable(ref tempVarName, Rplace);
                intermediateCodeGenerator.GenerateThreeAddressCodeSegment(ref code, tempVarName, Rplace);
                //tempVars.Push(temporaryVariable);
                AddOp();
                //stack.Push(code);
                Term(ref Tplace);
                //code = stack.Pop();
                code += Tplace.OffsetNotation;
                string value = Rplace.OffsetNotation;
                if (tempVarName != Rplace.OffsetNotation)
                {
                    Rplace.OffsetNotation = tempVarName;
                }
                else
                {
                    Rplace.OffsetNotation = value;
                }
                intermediateCodeGenerator.Emit(ref code);
                MoreTerm(ref Rplace);
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
                    BpOffsetNotation = $"_BP-{offset}";
                    if(value != null)
                    {
                        symbolTable.ConvertEntryToConstantEntry(entry, value);
                    }
                    else if(valueR != null)
                    {
                        symbolTable.ConvertEntryToConstantEntry(entry, valueR);
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
            ISymbolTableEntry Eplace = null;
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
                    intermediateCodeGenerator.tempVariableOffset = offset;
                    VariableDeclaration();
                    //temporaryVariableCounter = offset - 2;
                    SequenceOfStatements();
                    //temporaryVariableCounter = 0;
                    Match(Token.returnt);
                    Expr(ref Eplace);
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

