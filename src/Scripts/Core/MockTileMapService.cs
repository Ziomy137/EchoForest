using System.Collections.Generic;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// In-memory implementation of <see cref="ITileMapService"/> for use in NUnit tests.
/// Allows tests to configure tile walkability without requiring the Godot scene tree.
///
/// <c>TileData</c> is a Godot-runtime object and cannot be instantiated outside
/// the engine — <see cref="GetTileAtPosition"/> always returns <c>null</c> here.
/// For TileData queries, use the GUT integration tests instead.
/// </summary>
public sealed class MockTileMapService : ITileMapService
{
	private readonly HashSet<Vector2I> _walkableTiles = new();

	// ── Control API (used by tests to set up state) ───────────────────────────

	/// <summary>Registers a tile as walkable (<c>true</c>) or blocked (<c>false</c>).</summary>
	public void SetWalkable(Vector2I tilePos, bool walkable)
	{
		if (walkable) _walkableTiles.Add(tilePos);
		else _walkableTiles.Remove(tilePos);
	}

	/// <summary>Clears all registered tile state.</summary>
	public void Reset() => _walkableTiles.Clear();

	// ── ITileMapService ───────────────────────────────────────────────────────

	public Vector2I WorldToTile(Vector2 worldPos) =>
		IsometricMath.WorldToTile(worldPos);

	public Vector2 TileToWorld(Vector2I tilePos) =>
		IsometricMath.TileToWorld(tilePos);

	public bool IsWalkable(Vector2I tilePos) =>
		_walkableTiles.Contains(tilePos);

	/// <remarks>
	/// Always returns <c>null</c> — <c>TileData</c> cannot be constructed
	/// outside the Godot runtime. Test null-handling logic here;
	/// test actual tile data via GUT.
	/// </remarks>
	public TileData? GetTileAtPosition(Vector2 worldPos) => null;
}
