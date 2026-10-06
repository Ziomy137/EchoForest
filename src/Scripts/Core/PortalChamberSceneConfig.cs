using System;
using System.Linq;

namespace EchoForest.Core;

/// <summary>Pure-C# layout data for the Portal Chamber arena.</summary>
public static class PortalChamberSceneConfig
{
    public const int GridColumns = 30;
    public const int GridRows = 30;
    public const int TotalCells = GridColumns * GridRows;

    public const string SceneResPath = MainMenuConfig.PortalChamberScenePath;
    public const string TileSetResPath = CottageSceneConfig.TileSetResPath;
    public const string TileMapLayerName = "TileMapLayer";
    public const string BoundaryNodeName = "Boundary";
    public const string CameraNodeName = "Camera";
    public const string PlayerSpawnName = "PlayerSpawnPoint";
    public const string PortalFramesResPath = "res://src/Assets/Animations/portal_active_frames.tres";

    public const int SourceIdVoidFloor = 26;
    public const int SourceIdRuneFloor = 27;

    public const float WorldBoundaryLeft = -1024f;
    public const float WorldBoundaryRight = 1024f;
    public const float WorldBoundaryTop = -32f;
    public const float WorldBoundaryBottom = 1024f;

    public enum TransitionType
    {
        ExitToTower,
    }

    public readonly record struct TileZone(int Col, int Row, int Width, int Height)
    {
        public int TileCount => Width * Height;
        public bool Contains(int col, int row) =>
            col >= Col && col < Col + Width && row >= Row && row < Row + Height;
    }

    public readonly record struct GridPosition(int Col, int Row);
    public readonly record struct WorldPosition(float X, float Y);
    public readonly record struct PropPlacement(string FileName, int Col, int Row, bool IsBlocking);
    public readonly record struct TransitionEndpoint(TransitionType Type, string TargetArea, string SpawnPointId, GridPosition Position);

    public static readonly TileZone CentralPlatform = new(9, 9, 13, 13);
    public static readonly TileZone NorthBossArena = new(5, 2, 20, 8);
    public static readonly TileZone SouthEntrance = new(12, 23, 7, 6);

    public static readonly GridPosition CentralPortalPosition = new(15, 15);
    public static readonly GridPosition EntrancePortalPosition = new(15, 25);
    public static readonly GridPosition TowerEntranceSpawnPosition = new(15, 29);
    public static readonly string TowerEntranceSpawnPointId = "tower_entrance";
    public static readonly string TowerExitSpawnPointId = "portal_exit";
    public static readonly GridPosition TowerReturnTransitionPosition = EntrancePortalPosition;
    public static readonly GridPosition BossArenaPosition = new(15, 7);

    public static readonly GridPosition[] AnimatedPortalPositions =
    [
        CentralPortalPosition,
        EntrancePortalPosition,
    ];

    public static readonly PropPlacement[] Props =
    [
        new(PropRegistry.MagicPillar.FileName, 10, 11, true),
        new(PropRegistry.MagicPillar.FileName, 15, 10, true),
        new(PropRegistry.MagicPillar.FileName, 20, 11, true),
        new(PropRegistry.MagicPillar.FileName, 21, 15, true),
        new(PropRegistry.MagicPillar.FileName, 20, 19, true),
        new(PropRegistry.MagicPillar.FileName, 15, 20, true),
        new(PropRegistry.MagicPillar.FileName, 10, 19, true),
        new(PropRegistry.MagicPillar.FileName, 9, 15, true),
        new(PropRegistry.Chains.FileName, 5, 10, false),
        new(PropRegistry.Chains.FileName, 24, 10, false),
        new(PropRegistry.Chains.FileName, 5, 20, false),
        new(PropRegistry.Chains.FileName, 24, 20, false),
        new(PropRegistry.ChildCage.FileName, 15, 5, true),
    ];

    public static readonly TransitionEndpoint[] Transitions =
    [
        new(TransitionType.ExitToTower, MainMenuConfig.MagesTowerScenePath, MagesTowerSceneConfig.PortalExitSpawnPointId, TowerReturnTransitionPosition),
    ];

    public static WorldPosition GridToWorld(GridPosition position) => GridToWorld(position.Col, position.Row);

    public static WorldPosition GridToWorld(float col, float row) =>
        new((col - row) * TileRegistry.TileWidth / 2f, (col + row) * TileRegistry.TileHeight / 2f);

    public static string GetTileFileName(int col, int row)
    {
        if (!IsInBounds(col, row))
            throw new ArgumentOutOfRangeException(nameof(col));

        if (CentralPlatform.Contains(col, row) || SouthEntrance.Contains(col, row))
            return TileRegistry.RuneFloor.FileName;

        return TileRegistry.VoidFloor.FileName;
    }

    public static int GetSourceId(string fileName) => fileName switch
    {
        "tile_void_floor.png" => SourceIdVoidFloor,
        "tile_rune_floor.png" => SourceIdRuneFloor,
        _ => throw new ArgumentOutOfRangeException(nameof(fileName), fileName, "Unknown Portal Chamber tile."),
    };

    public static bool IsWalkable(int col, int row)
    {
        if (!IsInBounds(col, row))
            return false;

        var tileConfig = TileRegistry.GetByFileName(GetTileFileName(col, row));
        return tileConfig?.IsWalkable == true
            && !Props.Any(prop => prop.IsBlocking && prop.Col == col && prop.Row == row);
    }

    private static bool IsInBounds(int col, int row) =>
        col >= 0 && col < GridColumns && row >= 0 && row < GridRows;
}