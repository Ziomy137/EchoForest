using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>Pure-C# layout data for the 35 by 50 Forest Path area.</summary>
public static class ForestPathSceneConfig
{
    public const int GridColumns = 35;
    public const int GridRows = 50;
    public const int TotalCells = GridColumns * GridRows;

    public const string SceneResPath = MainMenuConfig.ForestPathScenePath;
    public const string TileSetResPath = CottageSceneConfig.TileSetResPath;
    public const string TileMapLayerName = "TileMapLayer";
    public const string BoundaryNodeName = "Boundary";
    public const string CameraNodeName = "Camera";
    public const string PlayerSpawnName = "PlayerSpawnPoint";

    public const int SourceIdWater = 5;
    public const int SourceIdForestFloor = 16;
    public const int SourceIdForestShadow = 17;
    public const int SourceIdMud = 18;
    public const int SourceIdRockSmall = 19;

    public const float WorldBoundaryLeft = -1600f;
    public const float WorldBoundaryRight = 1120f;
    public const float WorldBoundaryTop = -32f;
    public const float WorldBoundaryBottom = 1376f;
    public const float DenseTreeColliderWidth = 64f;
    public const float DenseTreeColliderHeight = 32f;

    /// <summary>Axis-aligned rectangle of cells within the Forest Path grid.</summary>
    public readonly record struct TileZone(int Col, int Row, int Width, int Height)
    {
        public int TileCount => Width * Height;
        public bool Contains(int col, int row) =>
            col >= Col && col < Col + Width && row >= Row && row < Row + Height;
    }

    /// <summary>Grid position for future encounter and pickup content.</summary>
    public readonly record struct GridPosition(int Col, int Row);

    /// <summary>File name and grid location for a Forest Path prop.</summary>
    public readonly record struct PropPlacement(string FileName, int Col, int Row, bool IsBlocking);

    public static readonly TileZone StreamZone = new(0, 24, GridColumns, 2);
    public static readonly TileZone WestCanopyShadow = new(0, 8, 9, 10);
    public static readonly TileZone EastCanopyShadow = new(26, 30, 9, 11);
    public static readonly TileZone RockyGround = new(8, 39, 5, 4);

    public static readonly GridPosition[] EnemyEncounterPositions =
    [
        new(14, 13),
        new(20, 37),
    ];

    public static readonly GridPosition OptionalItemPickupPosition = new(21, 31);

    public static readonly PropPlacement[] Props = CreateProps();

    /// <summary>Returns the center column of the three-cell winding mud path for a row.</summary>
    public static int GetPathColumn(int row)
    {
        if (row < 0 || row >= GridRows)
            throw new System.ArgumentOutOfRangeException(nameof(row));

        return row switch
        {
            < 10 => 17,
            < 20 => 16,
            < 30 => 17,
            < 40 => 18,
            _ => 17,
        };
    }

    /// <summary>Returns the highest-priority terrain tile for one Forest Path cell.</summary>
    public static string GetTileFileName(int col, int row)
    {
        if (IsMudPath(col, row)) return TileRegistry.Mud.FileName;
        if (StreamZone.Contains(col, row)) return TileRegistry.Water.FileName;
        if (RockyGround.Contains(col, row)) return TileRegistry.RockSmall.FileName;
        if (WestCanopyShadow.Contains(col, row) || EastCanopyShadow.Contains(col, row)) return TileRegistry.ForestShadow.FileName;
        return TileRegistry.ForestFloor.FileName;
    }

    /// <summary>Resolves a configured Forest Path tile to its shared TileSet source ID.</summary>
    public static int GetSourceId(string fileName) => fileName switch
    {
        "tile_water.png" => SourceIdWater,
        "tile_forest_floor.png" => SourceIdForestFloor,
        "tile_forest_shadow.png" => SourceIdForestShadow,
        "tile_mud.png" => SourceIdMud,
        "tile_rock_small.png" => SourceIdRockSmall,
        _ => throw new System.ArgumentOutOfRangeException(nameof(fileName), fileName, "Unknown Forest Path tile."),
    };

    private static bool IsMudPath(int col, int row) =>
        row >= 0 && row < GridRows && col >= GetPathColumn(row) - 1 && col <= GetPathColumn(row) + 1;

    private static PropPlacement[] CreateProps()
    {
        var props = new List<PropPlacement>();
        for (var row = 0; row < GridRows; row++)
        {
            var pathColumn = GetPathColumn(row);
            props.Add(new PropPlacement(PropRegistry.DenseTree.FileName, pathColumn - 6, row, true));
            props.Add(new PropPlacement(PropRegistry.DenseTree.FileName, pathColumn + 6, row, true));
        }

        props.Add(new PropPlacement(PropRegistry.FallenLog.FileName, 21, 28, true));
        props.Add(new PropPlacement(PropRegistry.Mushrooms.FileName, 13, 14, false));
        props.Add(new PropPlacement(PropRegistry.Mushrooms.FileName, 22, 33, false));
        props.Add(new PropPlacement(PropRegistry.Boulder.FileName, 12, 36, true));
        props.Add(new PropPlacement(PropRegistry.Boulder.FileName, 23, 42, true));
        return props.ToArray();
    }
}