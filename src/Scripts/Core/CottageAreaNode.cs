using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot <c>Node2D</c> that bootstraps the Cottage Exterior test area at runtime.
///
/// Reads all layout data from <see cref="CottageSceneConfig"/> (pure C#, tested)
/// and populates the TileMapLayer, props, and boundary at <c>_Ready()</c>.
/// No tile data needs to be baked into the <c>.tscn</c> file.
///
/// Excluded from NUnit code coverage — requires the Godot engine scene tree.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot Node2D wrapper — requires scene tree")]
public partial class CottageAreaNode : Node2D
{
    public override void _Ready()
    {
        PopulateTiles();
        SpawnProps();
        SetupBoundary();
    }

    // ─── Tile population ──────────────────────────────────────────────────────

    private void PopulateTiles()
    {
        var tileMap = GetNode<TileMapLayer>(CottageSceneConfig.TileMapLayerName);
        for (var row = 0; row < CottageSceneConfig.GridRows; row++)
        {
            for (var col = 0; col < CottageSceneConfig.GridColumns; col++)
            {
                var tile = CottageSceneConfig.GetTileFileName(col, row);
                var sourceId = CottageSceneConfig.GetSourceId(tile);
                tileMap.SetCell(new Vector2I(col, row), sourceId, Vector2I.Zero);
            }
        }
    }

    // ─── Prop spawning ────────────────────────────────────────────────────────

    private void SpawnProps()
    {
        // Props are added directly to the root so they share the same Y-sorted
        // parent as Player, enabling correct isometric depth interleaving.
        foreach (var placement in CottageSceneConfig.Props)
        {
            var config = PropRegistry.GetByFileName(placement.FileName);
            if (config is null) continue;

            var wx = CottageSceneConfig.TileToWorldX(placement.Col, placement.Row);
            var wy = CottageSceneConfig.TileToWorldY(placement.Col, placement.Row);

            // Y-sorter must be the parent so its ZIndex affects the rendered sprite.
            var sorter = new IsometricYSorterNode();
            sorter.Position = new Vector2(wx, wy);

            var sprite = new Sprite2D();
            sprite.Texture = GD.Load<Texture2D>(config.ResourcePath);
            sprite.Position = Vector2.Zero;

            sorter.AddChild(sprite);
            AddChild(sorter);

            // Blocking props also get a small collision circle
            if (placement.FileName == PropRegistry.Well.FileName
                || placement.FileName == PropRegistry.Tree.FileName
                || placement.FileName == PropRegistry.FencePost.FileName)
            {
                AddPropCollider(this, new Vector2(wx, wy), radius: 6f);
            }
        }
    }

    private static void AddPropCollider(Node2D parent, Vector2 worldPos, float radius)
    {
        var body = new StaticBody2D();
        body.CollisionLayer = 1u << (PhysicsLayers.World - 1);
        body.CollisionMask = 1u << (PhysicsLayers.World - 1);

        var shape = new CollisionShape2D();
        var circle = new CircleShape2D();
        circle.Radius = radius;
        shape.Shape = circle;

        body.AddChild(shape);
        body.GlobalPosition = worldPos;
        parent.AddChild(body);
    }

    // ─── Boundary walls ───────────────────────────────────────────────────────

    private void SetupBoundary()
    {
        var boundary = GetNode<StaticBody2D>(CottageSceneConfig.BoundaryNodeName);

        float cx = (CottageSceneConfig.WorldBoundaryLeft + CottageSceneConfig.WorldBoundaryRight) / 2f;
        float cy = (CottageSceneConfig.WorldBoundaryTop + CottageSceneConfig.WorldBoundaryBottom) / 2f;
        float w = CottageSceneConfig.WorldBoundaryRight - CottageSceneConfig.WorldBoundaryLeft + 128f;
        float h = CottageSceneConfig.WorldBoundaryBottom - CottageSceneConfig.WorldBoundaryTop + 128f;

        // North wall
        AddWallSegment(boundary, new Vector2(cx, CottageSceneConfig.WorldBoundaryTop - 32f), w, 64f);
        // South wall
        AddWallSegment(boundary, new Vector2(cx, CottageSceneConfig.WorldBoundaryBottom + 32f), w, 64f);
        // West wall
        AddWallSegment(boundary, new Vector2(CottageSceneConfig.WorldBoundaryLeft - 32f, cy), 64f, h);
        // East wall
        AddWallSegment(boundary, new Vector2(CottageSceneConfig.WorldBoundaryRight + 32f, cy), 64f, h);
    }

    private static void AddWallSegment(StaticBody2D parent, Vector2 center, float width, float height)
    {
        var shape = new CollisionShape2D();
        var rect = new RectangleShape2D();
        rect.Size = new Vector2(width, height);
        shape.Shape = rect;
        shape.Position = center;
        parent.AddChild(shape);
    }
}
