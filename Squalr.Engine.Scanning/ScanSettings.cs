namespace Squalr.Engine.Scanning
{
    using Squalr.Engine.Common.DataTypes;

    public static class ScanSettings
    {
        public static DataTypeBase DataType
        {
            get
            {
                return Properties.ScanSettings.Default.DataType;
            }

            set
            {
                Properties.ScanSettings.Default.DataType = value;
            }
        }
    }
}
