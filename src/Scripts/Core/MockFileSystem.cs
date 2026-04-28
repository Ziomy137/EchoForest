namespace EchoForest.Core;

/// <summary>
/// In-memory <see cref="IFileSystem"/> test double.
/// Stores a single file's content as a string so tests can verify Save/Load
/// round-trips without touching the real file system.
/// </summary>
public sealed class MockFileSystem : IFileSystem
{
    private string? _content;
    private readonly bool _simulateMissingFile;

    /// <param name="simulateMissingFile">
    ///   When <c>true</c>, <see cref="Exists"/> always returns <c>false</c>
    ///   and <see cref="ReadText"/> throws (simulates a missing file).
    ///   When <c>false</c> (default), the file "exists" once written, or
    ///   immediately if <paramref name="content"/> is provided.
    /// </param>
    /// <param name="content">
    ///   Optional pre-seeded file content. When provided the file is
    ///   treated as already existing (useful for corrupt-file tests).
    /// </param>
    public MockFileSystem(bool simulateMissingFile = false, string? content = null)
    {
        _simulateMissingFile = simulateMissingFile;
        _content = content;
    }

    public bool Exists(string path) => !_simulateMissingFile && _content is not null;

    public string ReadText(string path)
    {
        if (!Exists(path))
            throw new System.IO.FileNotFoundException($"Mock file not found: {path}");
        return _content!;
    }

    public void WriteText(string path, string content) => _content = content;

    public void Delete(string path) => _content = null;

    /// <summary>Returns the last value written via <see cref="WriteText"/>.</summary>
    public string? WrittenContent => _content;
}
