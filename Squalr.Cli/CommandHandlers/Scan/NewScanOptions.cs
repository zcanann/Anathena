namespace Squalr.Cli.CommandHandlers.Scan
{
    using CommandLine;
    using Squalr.Engine.Common.DataTypes;
    using System;

    [Verb("new", HelpText = "Starts a new scan")]
    public class NewScanOptions
    {
        public Int32 Handle()
        {
            DataTypeBase datatype = new DataTypeBase();

            if (String.IsNullOrWhiteSpace(this.DataTypeString))
            {
                this.DataTypeString = "int";
            }

            switch(this.DataTypeString.ToLower())
            {
                case "aob":
                    datatype.Type = DataTypeBase.ByteArray;
                    break;
                case "bool":
                    datatype.Type = DataTypeBase.Boolean;
                    break;
                case "sbyte":
                    datatype.Type = DataTypeBase.SByte;
                    break;
                case "short":
                case "int16":
                    datatype.Type = DataTypeBase.Int16;
                    break;
                case "int":
                case "int32":
                    datatype.Type = DataTypeBase.Int32;
                    break;
                case "long":
                case "int64":
                    datatype.Type = DataTypeBase.Int64;
                    break;
                case "byte":
                    datatype.Type = DataTypeBase.Byte;
                    break;
                case "ushort":
                case "uint16":
                    datatype.Type = DataTypeBase.Byte;
                    break;
                case "uint":
                case "uint32":
                    datatype.Type = DataTypeBase.UInt32;
                    break;
                case "ulong":
                case "uint64":
                    datatype.Type = DataTypeBase.UInt64;
                    break;
                case "float":
                case "single":
                    datatype.Type = DataTypeBase.Single;
                    break;
                case "double":
                    datatype.Type = DataTypeBase.Double;
                    break;
                case "string":
                    datatype.Type = DataTypeBase.String;
                    break;
                case "char":
                    datatype.Type = DataTypeBase.Char;
                    break;
                default:
                    Console.WriteLine("Unknown data type '" + this.DataTypeString + "', defaulting to int");
                    datatype.Type = DataTypeBase.Int32;
                    break;
            }

            return 0;
        }

        [Option('d', "data-type", Required = false, HelpText = "The data type of the scan. Choices: aob, bool, sbyte, int16 (short), int32 (int), int64 (long), byte, uint16 (ushort), uint32 (uint), uint64 (ulong), float (single), double, string, char")]
        public String DataTypeString { get; private set; }

        [Option('e', "encoding", Required = false, HelpText = "The string encoding type, if the data type is a string. Choices: ascii, unicode")]
        public String Encoding { get; private set; }
    }
    //// End class
}
//// End namespace
