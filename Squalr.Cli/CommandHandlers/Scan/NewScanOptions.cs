namespace Squalr.Cli.CommandHandlers.Scan
{
    using CommandLine;
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Scanning.Scanners;
    using Squalr.Engine.Scanning.Snapshots;
    using System;

    [Verb("new", HelpText = "Starts a new scan")]
    public class NewScanOptions
    {
        public Int32 Handle()
        {
            DataTypeBase dataType = new DataTypeBase();

            if (String.IsNullOrWhiteSpace(this.DataTypeString))
            {
                this.DataTypeString = "int";
            }

            switch(this.DataTypeString.ToLower())
            {
                case "aob":
                    dataType.Type = DataTypeBase.ByteArray;
                    break;
                case "bool":
                    dataType.Type = DataTypeBase.Boolean;
                    break;
                case "sbyte":
                    dataType.Type = DataTypeBase.SByte;
                    break;
                case "short":
                case "int16":
                    dataType.Type = DataTypeBase.Int16;
                    break;
                case "int":
                case "int32":
                    dataType.Type = DataTypeBase.Int32;
                    break;
                case "long":
                case "int64":
                    dataType.Type = DataTypeBase.Int64;
                    break;
                case "byte":
                    dataType.Type = DataTypeBase.Byte;
                    break;
                case "ushort":
                case "uint16":
                    dataType.Type = DataTypeBase.Byte;
                    break;
                case "uint":
                case "uint32":
                    dataType.Type = DataTypeBase.UInt32;
                    break;
                case "ulong":
                case "uint64":
                    dataType.Type = DataTypeBase.UInt64;
                    break;
                case "float":
                case "single":
                    dataType.Type = DataTypeBase.Single;
                    break;
                case "double":
                    dataType.Type = DataTypeBase.Double;
                    break;
                case "string":
                    dataType.Type = DataTypeBase.String;
                    break;
                case "char":
                    dataType.Type = DataTypeBase.Char;
                    break;
                default:
                    Console.WriteLine("Unknown data type '" + this.DataTypeString + "', defaulting to int");
                    dataType.Type = DataTypeBase.Int32;
                    break;
            }

            SessionManager.Session.SnapshotManager.ClearSnapshots();

            // Collect values
            TrackableTask<Snapshot> valueCollectorTask = ValueCollector.CollectValues(
                SessionManager.Session.SnapshotManager.GetActiveSnapshotCreateIfNone(SessionManager.Session.OpenedProcess, dataType),
                TrackableTask.UniversalIdentifier);

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
