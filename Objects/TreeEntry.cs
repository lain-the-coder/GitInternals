using System;
using System.Collections.Generic;

namespace GitInternals.Objects
{
    // Represents a single entry in a tree (one file or subdirectory)
    public class TreeEntry
    {
        public string? Mode { get; set; }  // "100644", "040000", etc.
        public string? Name { get; set; }  // "app.js", "src", etc.
        public string? Hash { get; set; }  // "736653821452..." (40 chars hex)
        public string? Type { get; set; }  // "blob" or "tree"
    }
}