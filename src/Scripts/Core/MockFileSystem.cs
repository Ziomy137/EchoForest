namespace EchoForest.Core;

/// <summary>
/// In-memory <see cref="IFileSystem"/> test double.
/// Stores a single file's content as a string so tests can verify Save/Load
/// round-trips without touching the real file system.
/// </summary>
public sealed class MockFileSystem : IFileSystem
{
    private string? _content;
    private readonly bool _fileExists;

    /// <param name="fileExists">
    ///   When <c>false</c>, <see cref="Exists"/> always returns <c>false</c>
    ///   and <see cref="ReadText"/> throws (simulates missing file).
    ///   When <c>true</c> (default), the file "exists" once written, or
    ///   immediately if <paramref name="content"/> is provided.
    /// </param>
    /// <param name="content">
    ///   Optional pre-seeded file content. When provided the file is
    ///   treated as already existing (useful for corrupt-file tests).
    /// </param>
    public MockFileSystem(bool fileExists = true, string? content = null)
    {
        _fileExists = fileExists;
        _content = content;
    }

    public bool Exists(string path) => _fileExists && _content is not null;

    public string ReadText(string path)
    {
        if (!Exists(path))
            throw new System.IO.FileNotFoundException($"Mock file not found: {path}");
        return _content!;
    }

    public void WriteText(string path, string content) => _content = content;

    /// <summary>Returns the last value written via <see cref="WriteText"/>.</summary>
    public string? WrittenContent => _content;
}
