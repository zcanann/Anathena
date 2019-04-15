namespace Squalr.Engine.Common
{
    using System;
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// Static class to help with compression and decompression.
    /// </summary>
    public static class Compression
    {
        /// <summary>
        /// Compresses the provided bytes via gzip.
        /// </summary>
        /// <param name="bytes">The bytes to compress.</param>
        /// <returns>The compressed bytes.</returns>
        public static Byte[] Compress(Byte[] bytes)
        {
            using (MemoryStream memoryStreamInput = new MemoryStream(bytes))
            {
                using (MemoryStream memoryStreamOutput = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(memoryStreamOutput, CompressionMode.Compress))
                    {
                        memoryStreamInput.CopyTo(gzipStream);
                    }

                    return memoryStreamOutput.ToArray();
                }
            }
        }

        /// <summary>
        /// Decompresses the provided bytes via gzip.
        /// </summary>
        /// <param name="bytes">The bytes to decompress.</param>
        /// <returns>The decompressed bytes.</returns>
        public static Byte[] Decompress(Byte[] bytes)
        {
            using (MemoryStream memoryStreamInput = new MemoryStream(bytes))
            {
                using (MemoryStream memoryStreamOutput = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(memoryStreamInput, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(memoryStreamOutput);
                    }

                    return memoryStreamOutput.ToArray();
                }
            }
        }
    }
    //// End class
}
//// End namespace