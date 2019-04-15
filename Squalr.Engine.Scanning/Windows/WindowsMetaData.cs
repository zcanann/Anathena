namespace Squalr.Engine.Scanning.Windows
{
    using PeNet;
    using PeNet.Structures;
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Common.Logging;
    using Squalr.Engine.Scanning.Snapshots;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Class for getting static metadata in a remote process.
    /// </summary>
    internal class WindowsMetaData : IMetaData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsAdapter"/> class.
        /// </summary>
        public WindowsMetaData()
        {
        }

        /// <summary>
        /// Gets or sets a reference to the target process.
        /// </summary>
        public Process ExternalProcess { get; set; }

        private static Dictionary<String, PeFile> PeCache = new Dictionary<string, PeFile>();

        private static Object CacheLock = new Object();

        public IList<SnapshotRegion> GetDataSegments(UInt64 moduleBase, String modulePath)
        {
            List<SnapshotRegion> regions = new List<SnapshotRegion>();

            // Normalize module path format to avoid caching on variants
            DirectoryInfo modulePathInfo = new DirectoryInfo(modulePath);
            modulePath = modulePathInfo.ToString();

            try
            {
                PeFile file;

                lock (WindowsMetaData.CacheLock)
                {
                    if (WindowsMetaData.PeCache.ContainsKey(modulePath))
                    {
                        file = WindowsMetaData.PeCache[modulePath];
                    }
                    else
                    {
                        file = new PeFile(modulePath);
                    }
                }

                foreach (IMAGE_SECTION_HEADER section in file.ImageSectionHeaders)
                {
                    const UInt32 IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040;
                    const UInt32 IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080;

                    // TODO: Not sure whether these are really data
                    const UInt32 IMAGE_SCN_LNK_COMDAT = 0x00001000;
                    const UInt32 IMAGE_SCN_GPREL = 0x00008000;

                    if ((section.Characteristics & IMAGE_SCN_CNT_INITIALIZED_DATA) != 0 ||
                        (section.Characteristics & IMAGE_SCN_CNT_UNINITIALIZED_DATA) != 0 ||
                        (section.Characteristics & IMAGE_SCN_LNK_COMDAT) != 0 ||
                        (section.Characteristics & IMAGE_SCN_GPREL) != 0)
                    {
                        Byte[] data = new Byte[section.SizeOfRawData];

                        using (BinaryReader reader = new BinaryReader(new FileStream(modulePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            reader.BaseStream.Seek(section.PointerToRawData, SeekOrigin.Begin);
                            reader.Read(data, 0, (Int32)section.SizeOfRawData);
                        }

                        ReadGroup readGroup = new ReadGroup(moduleBase + section.VirtualAddress, data, file.Is32Bit ? DataType.UInt32 : DataType.UInt64, file.Is32Bit ? 4 : 8);
                        regions.Add(new SnapshotRegion(readGroup, 0, data.Length));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error parsing the PE of module: " + (modulePath ?? "{null}"), ex);
            }

            return regions;
        }
    }
    //// End class
}
//// End namespace