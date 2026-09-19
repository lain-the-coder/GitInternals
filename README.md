# GitInternals - A Git Object Reader

A command-line tool built in C# that reads and parses Git's internal object storage format, demonstrating deep understanding of how Git works under the hood.

## Project Overview

This project implements Git's core object reading mechanisms from scratch, handling blob objects, tree objects, commits, and commit history traversal. Built to understand Git's internal architecture beyond the porcelain commands.

## Features

- **Blob Reader** - Decompress and display file content from Git objects
- **SHA-1 Hasher** - Calculate Git object hashes (equivalent to `git hash-object`)
- **Tree Parser** - Parse binary tree objects to display directory structures
- **Commit Parser** - Read and display commit metadata
- **History Walker** - Traverse commit history by following parent pointers

## Technical Implementation

### Core Concepts Demonstrated

**1. Zlib Compression/Decompression**
- All Git objects are compressed with Zlib (Deflate algorithm)
- Custom decompression handling skipping 2-byte Zlib header
- Uses .NET's `DeflateStream`

**2. Binary Format Parsing**
- Tree objects contain mixed text and binary data (20-byte SHA-1 hashes)
- Byte-by-byte parsing without converting to string (prevents corruption)
- Handling fixed-length binary fields (20-byte hashes)

**3. SHA-1 Hashing**
- Git hashes the full object format: `<type> <size>\0<content>`
- Hash calculated BEFORE compression
- Understanding content-addressable storage

**4. Linked List Traversal**
- Commits form a linked list via parent pointers
- History walking by following parent references
- Detecting root commits (no parent)

### Git Object Format

All objects follow the same storage pattern:
```
[Compressed with Zlib]
  ↓
[Decompressed: "<type> <size>\0<content>"]
  ↓
[Stored at: .git/objects/XX/YYYYYY...]
```

**Object Types:**
- **Blob** - File content (pure text)
- **Tree** - Directory listing (mixed text + binary hashes)
- **Commit** - Snapshot metadata (text format)

## Prerequisites

- .NET 8.0 SDK
- Git repository to read from
- WSL (Windows Subsystem for Linux) if accessing Linux-based repos from Windows

## Usage

### Configuration

Update the repository path in `Program.cs`:
```csharp
static string GitRepoPath = @"path\to\your\repo\.git";
```

### Commands

**Read a blob object:**
```bash
dotnet run read-blob <blob-hash>
```

**Calculate file hash:**
```bash
dotnet run hash-object <filepath>
```

**Read a tree object:**
```bash
dotnet run read-tree <tree-hash>
```

**Read a commit object:**
```bash
dotnet run read-commit <commit-hash>
```

**Walk commit history:**
```bash
dotnet run log <branch-name>
```

### Example Output

**Blob Reader:**
```
Object Type: blob
Size: 87 bytes

Content:
─────────────────────────────────────────
function greet(name) {
  console.log("Hello, " + name + "!");
}

greet("Git Internals");
─────────────────────────────────────────
```

**Tree Parser:**
```
Entries:
─────────────────────────────────────────────────────────────────────────────
100644 blob 8c40a0b4022a385e2af78ef804220fb61bdc0829    app.js
100644 blob a011cd6c0c517b785d9d270d0313e1e7fd3e5409    config.js
100644 blob 3b72cd7af624153d0e2b55c4ba86ea07b92eb055    utils.js
─────────────────────────────────────────────────────────────────────────────
```

**History Walker:**
```
commit 547497882b6b2133ae24ab43e58d3edde00bd6ed
    Updated version to 2.0.0

commit 0892c5aa64769cc48d259a3a1b7ec8761995c348
    Second commit message

commit c2b6d738e04417b6cff10ea9712a5b01b10c666f
    First commit

Total commits: 3
```

## Project Structure
```
GitInternals/
├── Program.cs              # Main CLI and command implementations
├── Utils/
│   ├── ZlibHelper.cs      # Zlib decompression
│   └── HashHelper.cs      # SHA-1 hashing
└── Objects/
    └── TreeEntry.cs       # Tree entry data structure
```

## Key Learnings

### 1. Storage vs. Logical Format
- Git stores objects compressed (physical layer)
- But hashes the uncompressed format (logical layer)
- Compression is just storage optimization, hash represents content

### 2. Binary Data Handling
- Can't blindly convert all Git objects to strings
- Tree objects have binary hashes that corrupt if converted
- Must parse byte arrays directly for mixed formats

### 3. Content-Addressable Storage
- Same content = same hash, regardless of filename
- Deduplication at object level
- Hash represents object identity

### 4. Git's Object Model
- Everything is an object (blob, tree, commit, tag)
- All stored identically (compression, hashing, storage)
- Only header determines object type

### 5. Commit History as Data Structure
- Commits form a singly-linked list
- Each commit points to parent (previous commit)
- Root commits have no parent (end of list)
- History traversal = following pointers

## Verification

Compare outputs with Git's built-in commands:
```bash
# Verify blob reading
git cat-file -p <hash>

# Verify hash calculation
git hash-object <file>

# Verify tree parsing
git cat-file -p <tree-hash>

# Verify commit parsing
git cat-file -p <commit-hash>

# Verify history
git log --oneline
```

## Technical Skills Demonstrated

- Low-level data format parsing (binary protocols)
- File I/O and byte manipulation
- Compression/decompression (Zlib/Deflate)
- Cryptographic hashing (SHA-1)
- Data structure traversal (linked lists)
- Binary vs. text encoding (UTF-8, hex conversion)
- Object-oriented design (separation of concerns)
- CLI application development
