namespace EchoForest.Core;

/// <summary>
/// Abstracts application-level lifecycle calls (quit, restart) so that
/// controllers remain testable without triggering real OS operations.
/// </summary>
public interface IApplicationController
{
    /// <summary>Terminates the application.</summary>
    void Quit();
}
