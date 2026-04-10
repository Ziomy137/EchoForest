using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot-serialisable resource that holds the 16 approved EchoForest palette
/// colors. Designed to be loaded in the Godot editor and attached to shaders or
/// scripts that need runtime access to the full color list.
///
/// The <see cref="Colors"/> array is pre-populated in <c>palette.tres</c> and
/// mirrors the constants defined in <see cref="Palette"/>.
///
/// Marked <c>[GlobalClass]</c> so Godot's editor and importer can resolve the
/// type without walking the full C# assembly on every project load.
/// </summary>
[GlobalClass]
[ExcludeFromCodeCoverage]
public partial class PaletteResource : Resource
{
    /// <summary>All 16 approved palette colors, in the same order as <see cref="Palette.All"/>.</summary>
    [Export]
    public Color[] Colors { get; set; } = System.Array.Empty<Color>();
}
