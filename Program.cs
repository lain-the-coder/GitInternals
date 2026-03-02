using System;

namespace GitInternals
{
    class Program
    {
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
            }
            static void ReadBlob(string[] args)
            {
                Console.WriteLine("TODO: Read blob");
            }
            static void HashObject(string[] args)
            {
                Console.WriteLine("TODO: Hash object");
            }
            static void ReadTree(string[] args)
            {
                Console.WriteLine("TODO: Read tree");
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