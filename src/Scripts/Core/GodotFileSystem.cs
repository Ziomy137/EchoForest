using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Production <see cref="IFileSystem"/> that delegates to Godot's
/// <c>FileAccess</c> API, resolving <c>user://</c> paths to the OS-specific
/// user data directory.
///
/// Excluded from NUnit code coverage — requires the Godot runtime.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot FileAccess wrapper — requires Godot runtime")]
public sealed class GodotFileSystem : IFileSystem
{
    public bool Exists(string path) => FileAccess.FileExists(path);

    public string ReadText(string path)
    {
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        return file.GetAsText();
    }

    public void WriteText(string path, string content)
    {
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        file.StoreString(content);
    }

    public void Delete(string path)
    {
        if (FileAccess.FileExists(path))
            DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(path));
    }
}
