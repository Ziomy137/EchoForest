namespace EchoForest.Core;

/// <summary>
/// Abstracts the save-file persistence layer for use in controllers and tests.
/// </summary>
public interface ISaveService
{
    /// <summary>Returns <c>true</c> if at least one save file exists on disk.</summary>
    bool HasSaveFile();
}
