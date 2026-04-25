namespace EchoForest.Core;

/// <summary>
/// In-memory implementation of <see cref="IApplicationController"/> for use in NUnit tests.
/// Records whether <see cref="Quit"/> was called instead of exiting the process.
/// </summary>
public sealed class MockApplicationController : IApplicationController
{
    /// <summary>Whether <see cref="Quit"/> has been called at least once.</summary>
    public bool QuitWasCalled { get; private set; }

    /// <inheritdoc/>
    public void Quit() => QuitWasCalled = true;
}
