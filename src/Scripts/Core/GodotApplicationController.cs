using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Production implementation of <see cref="IApplicationController"/> backed by
/// Godot's <c>SceneTree.Quit()</c>. Requires the Godot scene tree.
///
/// Excluded from NUnit code coverage — requires scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot SceneTree wrapper — requires scene tree")]
public sealed class GodotApplicationController : IApplicationController
{
    public void Quit()
    {
        if (Engine.GetMainLoop() is SceneTree tree)
            tree.Quit();
    }
}
