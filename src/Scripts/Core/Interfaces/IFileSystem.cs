namespace EchoForest.Core;

/// <summary>
/// Abstracts file-system I/O so <see cref="ConfigService"/> can be tested
/// without touching the real file system or Godot's <c>user://</c> path.
/// </summary>
public interface IFileSystem
{
    /// <summary>Returns <c>true</c> if the file at <paramref name="path"/> exists.</summary>
    bool Exists(string path);

    /// <summary>Reads the entire text content of the file at <paramref name="path"/>.</summary>
    string ReadText(string path);

    /// <summary>Writes <paramref name="content"/> to the file at <paramref name="path"/>, creating it if needed.</summary>
    void WriteText(string path, string content);

    /// <summary>Deletes the file at <paramref name="path"/>. No-op if the file does not exist.</summary>
    void Delete(string path);
}
