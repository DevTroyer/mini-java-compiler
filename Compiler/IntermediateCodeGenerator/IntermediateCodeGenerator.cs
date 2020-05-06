using System.IO;

namespace Compiler
{
    public class IntermediateCodeGenerator : Resources
    {
        public StreamWriter tacFile;
        public int tempVariableOffset { get; set; }

        public IntermediateCodeGenerator()
        {
            tempVariableOffset = 2;
        }

        public void SetupTacFile(string tacFilename)
        {
            tacFilename = Path.ChangeExtension(tacFilename, ".tac");
            string tacFilePath = Path.Combine(Directory.GetCurrentDirectory(), tacFilename);

            try
            {
                FileStream fileStream = File.Create(tacFilePath);
                fileStream.Close();
            }
            catch
            {
                ExceptionHandler.ThrowFileExistsException(tacFilename);
            }

            tacFile = File.AppendText(tacFilename);
            tacFile.AutoFlush = true;
        }

        public void CreateTempVariable(ref string tempVarName, ISymbolTableEntry entry)
        {
            Variable var = entry as Variable;

            tempVarName = $"_bp-{sizeOfLocalMethodVariables + tempVariableOffset}";

            if(var != null)
            {
                tempVariableOffset += 2;
            }

            //if (var != null && var.TypeOfEntry != EntryType.tableEntry)
            //{
            //    tempVariableOffset += 2;
            //}
            //else
            //{
            //    CalculateSize();
            //}
        }

        public void GenerateThreeAddressCodeSegment(ref string code, string tempVarName, ISymbolTableEntry Rplace)
        {
            code = $"{tempVarName} = {Rplace.OffsetNotation} {lexeme.ToString()} ";
        }

        public void GenerateFinalExpression(ISymbolTableEntry entry, ISymbolTableEntry Eplace, ref string code)
        {
            code = $"{entry.OffsetNotation} = {Eplace.OffsetNotation}";
            Emit(ref code);
        }

        public void Emit(ref string threeAddressCodeLine)
        {
            tacFile.WriteLine(threeAddressCodeLine);
            threeAddressCodeLine = "";
        }

        public void Emit(string threeAddressCodeLine)
        {
            if(threeAddressCodeLine != "")
                tacFile.WriteLine(threeAddressCodeLine);
        }

        public void GenerateTempExpressionTAC(ref ISymbolTableEntry Tplace)
        {
            string tempVarName = "";
            CreateTempVariable(ref tempVarName, Tplace);
            Emit($"{tempVarName} = {Tplace.OffsetNotation}");
            Tplace.OffsetNotation = tempVarName;
        }

        private void CalculateSize()
        {
            switch (dataType)
            {
                case DataType.floatType:
                    tempVariableOffset += 4;
                    break;
                case DataType.intType:
                    tempVariableOffset += 2;
                    break;
                case DataType.booleanType:
                    tempVariableOffset += 1;
                    break;
            }
        }

        // Old implementation before refactor: keep for future reference

        //public string CreateTempVariable()
        //{
        //    temporaryVariableCounter += (int)dataType;

        //    if (temporaryVariableCounter > 99)
        //    {
        //        ExceptionHandler.ThrowVariableOverflowException();
        //    }

        //    return temporaryVariable = $"_BP-{temporaryVariableCounter}";
        //}

        //public string CalculateParameterOffsetNotation(ISymbolTableEntry entry)
        //{
        //    int size = CalculateSize();
        //    string bpLocation = string.Format($"_BP+{offset + size}");
        //    return bpLocation;
        //}

        //public string CalculateLocalVariableOffsetNotation(ISymbolTableEntry entry)
        //{
        //    int size = CalculateSize();
        //    string bpLocation = string.Format($"_BP-{offset + size - 2}");
        //    return bpLocation;
        //}
    }
}
