using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace GitInternals.Utils
{
    public static class ZlibHelper
    {
        public static byte[] Decompress(byte[] compressedData)
        {
            // Step 1: Skip first 2 bytes (header); convert bytes to stream
            using (var compressedStream = new MemoryStream(compressedData, 2, compressedData.Length - 2))
            {
                // Step 2: Decompress with DeflateStream (straw); 
                using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                {
                    // Step 3: Write decompressed output; deflateStream needs to know where to decompress and copy to; use MemoryStream for output
                    using (var decompressedStream = new MemoryStream())
                    {
                        deflateStream.CopyTo(decompressedStream);
                        //Convert stream to byte array
                        return decompressedStream.ToArray();
                    }
                }
            }
        }
    }
}