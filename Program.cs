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
            Console.WriteLine("$Commands recieved: {args[0]}");
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
        }
    }
}