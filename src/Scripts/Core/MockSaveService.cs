namespace EchoForest.Core;

/// <summary>
/// In-memory implementation of <see cref="ISaveService"/> for use in NUnit tests.
/// </summary>
public sealed class MockSaveService : ISaveService
{
    private readonly bool _hasSave;

    /// <param name="hasSave">Whether <see cref="HasSaveFile"/> should return <c>true</c>.</param>
    public MockSaveService(bool hasSave = false) => _hasSave = hasSave;

    /// <inheritdoc/>
    public bool HasSaveFile() => _hasSave;
}
