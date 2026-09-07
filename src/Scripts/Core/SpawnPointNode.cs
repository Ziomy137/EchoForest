using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>Named marker used as an entry point after an area transition.</summary>
[ExcludeFromCodeCoverage(Justification = "Godot Marker2D wrapper - requires scene tree")]
public partial class SpawnPointNode : Marker2D
{
    /// <summary>Stable identifier referenced by an <see cref="AreaTransitionNode"/>.</summary>
    [Export]
    public string SpawnPointId { get; set; } = string.Empty;
}