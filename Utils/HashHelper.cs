using System;
using System.Security.Cryptography;
using System.Text;

namespace GitInternals.Utils
{
    public static class HashHelper
    {
        public static string ComputeSHA1(byte[] data)
        {
            // Calculate SHA-1 hash
            byte[] hashBytes = SHA1.HashData(data);
            // Input:  [98, 108, 111, 98, 32, 56, 55, ...]  (bytes)
            // Output: [115, 102, 101, 83, 130, ...]        (20-byte hash)

            // Convert to hex string
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}