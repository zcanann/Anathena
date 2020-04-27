namespace Squalr.Cli.CommandHandlers.Scan
{
    using CommandLine;
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Scanning;
    using Squalr.Engine.Scanning.Scanners;
    using Squalr.Engine.Scanning.Snapshots;
    using System;

    [Verb("new", HelpText = "Starts a new scan")]
    public class NewScanOptions
    {
        public Int32 Handle()
        {
            if (String.IsNullOrWhiteSpace(this.DataTypeString))
            {
                this.DataTypeString = "int";
            }

            switch(this.DataTypeString.ToLower())
            {
                case "aob":
                    ScanSettings.DataType = DataTypeBase.ByteArray;
                    break;
                case "bool":
                    ScanSettings.DataType = DataTypeBase.Boolean;
                    break;
                case "sbyte":
                    ScanSettings.DataType = DataTypeBase.SByte;
                    break;
                case "i16":
                case "short":
                case "int16":
                    ScanSettings.DataType = DataTypeBase.Int16;
                    break;
                case "i":
                case "int":
                case "int32":
                    ScanSettings.DataType = DataTypeBase.Int32;
                    break;
                case "l":
                case "i64":
                case "long":
                case "int64":
                    ScanSettings.DataType = DataTypeBase.Int64;
                    break;
                case "b":
                case "byte":
                    ScanSettings.DataType = DataTypeBase.Byte;
                    break;
                case "ui16":
                case "ushort":
                case "uint16":
                    ScanSettings.DataType = DataTypeBase.Byte;
                    break;
                case "ui32":
                case "uint":
                case "uint32":
                    ScanSettings.DataType = DataTypeBase.UInt32;
                    break;
                case "ui64":
                case "ul":
                case "ulong":
                case "uint64":
                    ScanSettings.DataType = DataTypeBase.UInt64;
                    break;
                case "f":
                case "float":
                case "single":
                    ScanSettings.DataType = DataTypeBase.Single;
                    break;
                case "d":
                case "double":
                    ScanSettings.DataType = DataTypeBase.Double;
                    break;
                case "str":
                case "string":
                    ScanSettings.DataType = DataTypeBase.String;
                    break;
                case "char":
                    ScanSettings.DataType = DataTypeBase.Char;
                    break;
                default:
                    Console.WriteLine("Unknown data type '" + this.DataTypeString + "', defaulting to int");
                    ScanSettings.DataType = DataTypeBase.Int32;
                    break;
            }

            SessionManager.Session.SnapshotManager.ClearSnapshots();

            Console.WriteLine("Data type for new scan set to: " + ScanSettings.DataType.ToString());

            // Collect values
            TrackableTask<Snapshot> valueCollectorTask = ValueCollector.CollectValues(
                SessionManager.Session.SnapshotManager.GetActiveSnapshotCreateIfNone(SessionManager.Session.OpenedProcess, ScanSettings.DataType),
                TrackableTask.UniversalIdentifier);

            valueCollectorTask.OnCompletedEvent += ((completedValueCollectionTask) =>
            {
                Console.WriteLine();
            });

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
