using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// In-memory <see cref="IFileSystem"/> test double.
/// Stores file contents in a <see cref="Dictionary{TKey,TValue}"/> keyed by path,
/// so tests can verify per-path Save/Load round-trips and slot isolation
/// without touching the real file system.
/// </summary>
public sealed class MockFileSystem : IFileSystem
{
    private readonly Dictionary<string, string> _files;

    /// <summary>Creates an empty file system (no files exist).</summary>
    public MockFileSystem()
    {
        _files = new Dictionary<string, string>();
    }

    /// <param name="files">
    ///   Pre-seeded path-to-content map. When provided, the corresponding paths
    ///   are treated as already existing (useful for corrupt-file tests).
    ///   The dictionary is copied so the caller's copy is not mutated.
    /// </param>
    public MockFileSystem(Dictionary<string, string> files)
    {
        _files = new Dictionary<string, string>(files);
    }

    public bool Exists(string path) => _files.ContainsKey(path);

    public string ReadText(string path)
    {
        if (!_files.TryGetValue(path, out var content))
            throw new System.IO.FileNotFoundException($"Mock file not found: {path}");
        return content;
    }

    public void WriteText(string path, string content) => _files[path] = content;

    public void Delete(string path) => _files.Remove(path);

    /// <summary>Returns the content written to <paramref name="path"/>, or <c>null</c> if absent.</summary>
    public string? GetContent(string path) => _files.TryGetValue(path, out var c) ? c : null;
}
