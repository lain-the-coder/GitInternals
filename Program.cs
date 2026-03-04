using System;
using System.IO;
using System.Text;
using GitInternals.Utils;

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
                case "verify-object":
                    VerifyObject(args);
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
                Console.WriteLine("  verify-object <hash>  Verify object integrity");
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
                
            }

            static void ReadCommit(string[] args)
            {
                Console.WriteLine("TODO: Read commit");
            }

            static void Log(string[] args)
            {
                Console.WriteLine("TODO: Log");
            }

            static void VerifyObject(string[] args)
            {
                Console.WriteLine("TODO: Verify object");
            }
        }
    }
}