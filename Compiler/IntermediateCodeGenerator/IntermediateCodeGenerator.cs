using System.IO;

namespace Compiler
{
    public class IntermediateCodeGenerator : Resources
    {
        public StreamWriter tacFile;

        public IntermediateCodeGenerator()
        {
            temporaryVariableCounter = 0;
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

        public string CreateTempVariable()
        {
            temporaryVariableCounter += (int)dataType;

            if (temporaryVariableCounter > 99)
            {
                ExceptionHandler.ThrowVariableOverflowException();
            }

            return temporaryVariable = $"_BP-{temporaryVariableCounter}";
        }

        public void GenerateThreeAddressCodeSegment()
        {
            code = $"{temporaryVariable} = {Tplace.OffsetNotation} {lexeme.ToString()} ";
        }

        public void GenerateFinalExpression(Variable entry)
        {
            code = $"{entry.OffsetNotation} = {temporaryVariable}";
        }

        public void Emit(string threeAddressCodeLine)
        {
            tacFile.WriteLine(threeAddressCodeLine);
        }

        public string CalculateParameterOffsetNotation(ISymbolTableEntry entry)
        {
            int size = CalculateSize();
            string bpLocation = string.Format($"_BP+{offset + size}");
            return bpLocation;
        }

        public string CalculateLocalVariableOffsetNotation(ISymbolTableEntry entry)
        {
            int size = CalculateSize();
            string bpLocation = string.Format($"_BP-{offset + size - 2}");
            return bpLocation;
        }

        private int CalculateSize()
        {
            int size = 2;
            switch (dataType)
            {
                case DataType.floatType:
                    size += 4;
                    break;
                case DataType.intType:
                    size += 2;
                    break;
                case DataType.booleanType:
                    size += 1;
                    break;
            }
            return size;
        }

        // CALL IMMEDIATELY AFTER CREATETEMPVARIABLE()

        //ISymbolTableEntry entry = new SymbolTableEntry(Token.idt, temporaryVariable, depth, EntryType.tableEntry);

        //symbolTable.Insert(entry);

        //symbolTable.ConvertEntryToVariableEntry(entry);





        //public string CalculateOffsetParameterNotation(ISymbolTableEntry entry)
        //{
        //    string bpLocation = string.Empty;
        //    if (entry.Depth == 1 || entry.TypeOfEntry == EntryType.methodEntry)
        //    {
        //        bpLocation = entry.Lexeme.ToString();
        //    }
        //    else
        //    {
        //        if (entry.TypeOfEntry == EntryType.varEntry && entry.isparamater ?)
        //            bpLocation = string.Format("_BP+{0}", entry.Offset);
        //        else
        //            bpLocation = string.Format("_BP-{0}", entry.Value.Offset);
        //    }

        //    return bpLocation;
        //}
    }
}
