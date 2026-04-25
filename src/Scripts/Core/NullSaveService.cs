namespace EchoForest.Core;

/// <summary>
/// Placeholder <see cref="ISaveService"/> used by <see cref="MainMenuNode"/>
/// until the real save system (S5-04) is implemented.
/// Always reports no save file exists.
/// </summary>
public sealed class NullSaveService : ISaveService
{
    /// <inheritdoc/>
    public bool HasSaveFile() => false;
}
