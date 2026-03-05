using System;
using System.IO;
using System.Text;
using GitInternals.Utils;
using GitInternals.Objects;
using System.Collections.Generic;

namespace GitInternals
{
    class Program
    {
        //Path to your git repository
        static string GitRepoPath = @"\\wsl.localhost\Ubuntu-22.04\home\dev\workspace\github.com\lain-the-coder\git-internals-deep-dive\.git";

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                showUsage();
                return;
            }

            string command = args[0];
                    
            switch (command)
            {
                case "read-blob":
                    ReadBlob(args);
                    break;
                case "hash-object":
                    HashObject(args);
                    break;
                case "read-tree":
                    ReadTree(args);
                    break;
                case "read-commit":
                    ReadCommit(args);
                    break;
                case "log":
                    Log(args);
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    showUsage();
                    break;
            }

            static void showUsage()
            {
                Console.WriteLine("GitInternals - A Git object reader");
                Console.WriteLine();
                Console.WriteLine("Commands:");
                Console.WriteLine("  read-blob <hash>      Read and display a blob object");
                Console.WriteLine("  hash-object <file>    Calculate SHA-1 hash of a file");
                Console.WriteLine("  read-tree <hash>      Read and display a tree object");
                Console.WriteLine("  read-commit <hash>    Read and display a commit object");
                Console.WriteLine("  log <branch>          Walk commit history");
                Console.WriteLine($"Git Repository: {GitRepoPath}");

            }

            static void ReadBlob(string[] args)
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: read-blob <hash>");
                    return;
                }
                string hash = args[1];
                Console.WriteLine($"Reading blob: {hash}");

                //Building file path from Hash
                string folder = hash.Substring(0, 2);
                string fileName = hash.Substring(2);
                string objectPath = Path.Combine(GitRepoPath, "objects", folder, fileName);
                Console.WriteLine($"Object path: {objectPath}");

                //Read file from path into byte array
                byte[] compressedData = File.ReadAllBytes(objectPath);

                //Decompress data
                byte[] decompressedData = ZlibHelper.Decompress(compressedData);

                //Convert bytes to string
                string fullContent = Encoding.UTF8.GetString(decompressedData);

                //Parse header and content(split at null byte \0)
                int nullByteSeparatorIndex = fullContent.IndexOf('\0');
                string header = fullContent.Substring(0, nullByteSeparatorIndex);
                string content = fullContent.Substring(nullByteSeparatorIndex + 1);

                //Parse/split header details by splitting blob and size
                string[] headerDetails = header.Split(' ');
                string type = headerDetails[0];
                int size = int.Parse(headerDetails[1]);

                //Display object details and content
                Console.WriteLine();
                Console.WriteLine($"Object Type: {type}");
                Console.WriteLine($"Size: {size} bytes");
                Console.WriteLine();
                Console.WriteLine("Content:");
                Console.WriteLine("─────────────────────────────────────────");
                Console.WriteLine(content);
                Console.WriteLine("─────────────────────────────────────────");

            }

            static void HashObject(string[] args)
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: hash-object <file>");
                    return;
                }
                //Get file path
                string filePath = args[1];
                
                //Read file into byte array
                byte[] contentBytes = File.ReadAllBytes(filePath);

                //Create Git blob header
                string header = $"blob {contentBytes.Length}\0";

                //Convert header to bytes
                byte[] headerBytes = Encoding.UTF8.GetBytes(header);

                //Combine header and content bytes
                //Allocate new byte array to hold header and content
                byte[] data = new byte[headerBytes.Length + contentBytes.Length];
                //Copy header bytes to new array
                Array.Copy(headerBytes, 0, data, 0, headerBytes.Length);    
                //Copy content bytes to new array after header
                Array.Copy(contentBytes, 0, data, headerBytes.Length, contentBytes.Length);

                //Calculate SHA-1 hash of combined data
                string hash = HashHelper.ComputeSHA1(data);

                // Display result
                Console.WriteLine($"File: {filePath}");
                Console.WriteLine($"SHA1: {hash}");
            }

            static void ReadTree(string[] args)
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: read-tree <hash>");
                    return;
                }
                string hash = args[1];
                Console.WriteLine($"Reading tree: {hash}");

                //Building file path from Hash
                string folder = hash.Substring(0, 2);
                string fileName = hash.Substring(2);
                string objectPath = Path.Combine(GitRepoPath, "objects", folder, fileName);
                Console.WriteLine($"Object path: {objectPath}");

                //Read and decompress data
                byte[] compressedData = File.ReadAllBytes(objectPath);
                byte[] decompressedData = ZlibHelper.Decompress(compressedData);

                Console.WriteLine("Decompressed successfully.");

                //Skip header - tree <size>\0
                int position = 0;
                while (decompressedData[position] != 0)
                {
                    position++;
                }
                position++; // Move past null byte

                Console.WriteLine($"Header skipped. Starting at position: {position}");

                //Empty List to store entries
                var entries = new List<TreeEntry>();
                Console.WriteLine("Total Decompressed bytes: " + decompressedData.Length);
                Console.WriteLine($"Total bytes to parse(actual data entry without header): {decompressedData.Length - position}");

                //Loop through decompressed data until we reach the end, parsing each entry
                while (position < decompressedData.Length)
                {
                    //Read Mode
                    int modestart = position; //modestart = 9

                    while (decompressedData[position] != ' ')
                    {
                        position++; // Move to next byte until we find a space, stops at null byte; since 1   0   0   6   4   4       a   p   p   .   j   s ; space between mode and filename
                    }

                    string mode = Encoding.UTF8.GetString(decompressedData, modestart, position - modestart); //Gets the 6 bytes [49, 48, 48, 48, 48, 48]; converts to string "100000" which is the mode(blob)

                    position++; // Move past space

                    //Read Filename
                    int namestart = position; //namestart = 16

                    while (decompressedData[position] != 0)
                    {
                        position++; // Move to next byte until we find a null byte since filename ends with null byte; since app.js\0
                    }

                    string name = Encoding.UTF8.GetString(decompressedData, namestart, position - namestart); //Gets the 6 bytes [97, 112, 112, 46, 106, 115]; converts to string "app.js" which is the filename

                    position++; // Move past null byte

                    //Read Hash
                    byte[] hashBytes = new byte[20]; //SHA-1 hash is 20 bytes in binary not hex; this will copy 20 bytes starting from position 23 to hashBytes array
                    Array.Copy(decompressedData, position, hashBytes, 0, 20);

                    position += 20; // Move position past the hash bytes

                    string hexHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); //Convert hashBytes to hex string

                    //Determine type of object
                    string type = mode == "040000" ? "tree" : "blob"; //040000 is tree, 100644 is blob

                    //Create TreeEntry object
                    entries.Add(new TreeEntry
                    {
                        Mode = mode,
                        Name = name,
                        Hash = hexHash,
                        Type = type
                    });
                }

                Console.WriteLine($"Parsed {entries.Count} entries!");

                Console.WriteLine();
                Console.WriteLine("Entries:");
                Console.WriteLine("─────────────────────────────────────────────────────────────────────────────");

                foreach (var entry in entries)
                {
                    Console.WriteLine($"{entry.Mode} {entry.Type,-4} {entry.Hash}    {entry.Name}");
                }

                Console.WriteLine("─────────────────────────────────────────────────────────────────────────────");
            }

            static void ReadCommit(string[] args)
            {
                //Same as ReadBlob
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: read-commit <hash>");
                    return;
                }
                string hash = args[1];
                string folder = hash.Substring(0, 2);
                string filename = hash.Substring(2);
                string filepath = Path.Combine(GitRepoPath, "objects", folder, filename);
                Console.WriteLine($"This is the filepath: {filepath}");
                byte[] commitByte = File.ReadAllBytes(filepath);
                byte[] commitDecompressed = ZlibHelper.Decompress(commitByte);
                string fullContent = Encoding.UTF8.GetString(commitDecompressed);
                int nullIndex = fullContent.IndexOf('\0');
                string header = fullContent.Substring(0, nullIndex);
                string content = fullContent.Substring(nullIndex + 1);
                string[] headers = header.Split(' ');
                string type = headers[0];
                string size = headers[1];

                Console.WriteLine($"Type: {type}");
                Console.WriteLine($"Size: {size}");
                Console.WriteLine();
                Console.WriteLine("Content:");
                Console.WriteLine(content);
            }

            static void Log(string[] args)
            {
                Console.WriteLine("TODO: Log");
            }
        }
    }
}