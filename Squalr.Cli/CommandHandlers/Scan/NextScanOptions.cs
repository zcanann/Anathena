namespace Squalr.Cli.CommandHandlers.Scan
{
    using CommandLine;
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Scanning.Scanners;
    using Squalr.Engine.Scanning.Scanners.Constraints;
    using Squalr.Engine.Scanning.Snapshots;
    using System;

    [Verb("next", HelpText = "Starts the next scan")]
    public class NextScanOptions
    {
        public Int32 Handle()
        {
            DataType dataType = DataType.Int32; // TODO: Fetch from settings
            ScanConstraint.ConstraintType constraintType = ScanConstraint.ConstraintType.Equal;

            if (String.IsNullOrWhiteSpace(this.Constraint))
            {
                // Default to equals
                this.Constraint = "e";
            }

            switch(this.Constraint.ToLower())
            {
                case "le":
                    break;
                case "l":
                    break;
                case "g":
                    break;
                case "ge":
                    break;
                case "e":
                    break;
                case "c":
                    break;
                case "u":
                    break;
                case "i":
                    break;
                case "d":
                    break;
                default:
                    Console.WriteLine("Unknown constraint type '" + this.Constraint + "', defaulting to equal");
                    break;
            }

            if (!SyntaxChecker.CanParseValue(dataType, this.Value))
            {
                Console.WriteLine("Failed to parse '" + this.Value + "' as data type " + dataType?.ToString());
                return -1;
            }

            ScanConstraint scanConstraints = new ScanConstraint(constraintType, Conversions.ParsePrimitiveStringAsPrimitive(dataType, this.Value), dataType);

            // Collect values
            TrackableTask<Snapshot> valueCollectorTask = ValueCollector.CollectValues(
                SessionManager.Session.SnapshotManager.GetActiveSnapshotCreateIfNone(SessionManager.Session.OpenedProcess, dataType),
                TrackableTask.UniversalIdentifier);

            // Perform manual scan on value collection complete
            valueCollectorTask.OnCompletedEvent += ((completedValueCollectionTask) =>
            {
                Snapshot snapshot = valueCollectorTask.Result;
                TrackableTask<Snapshot> scanTask = ManualScanner.Scan(
                    snapshot,
                    scanConstraints,
                    TrackableTask.UniversalIdentifier);

                SessionManager.Session.SnapshotManager.SaveSnapshot(scanTask.Result);
            });

            Console.WriteLine();
            Console.WriteLine();

            return 0;
        }

        [Value(0, MetaName = "value", HelpText = "The value for which to scan.")]
        public String Value { get; set; }

        [Value(1, MetaName = "constraint", Required = false, HelpText = "The constraint of the scan. The default is 'equals'. Options: le (less than or equals), l (less than), ge (greater than or equals), g (greater than), e (equals), c (changed), u (unchanged), i (increased), d (decreased)")]
        public String Constraint { get; set; }
    }
    //// End class
}
//// End namespace
