using System;
using System.Linq;

namespace EchoForest.Core;

/// <summary>Pure-C# layout data for the Mage's Tower exterior and ground floor.</summary>
public static class MagesTowerSceneConfig
{
    public const int GridColumns = 40;
    public const int GridRows = 24;
    public const int TotalCells = GridColumns * GridRows;

    public const string SceneResPath = MainMenuConfig.MagesTowerScenePath;
    public const string TileSetResPath = CitySceneConfig.TileSetResPath;
    public const string TileMapLayerName = "TileMapLayer";
    public const string BoundaryNodeName = "Boundary";
    public const string CameraNodeName = "Camera";
    public const string PlayerSpawnName = "PlayerSpawnPoint";

    public const int SourceIdCobblestone = CitySceneConfig.SourceIdCobblestone;
    public const int SourceIdCityStone = CitySceneConfig.SourceIdCityStone;
    public const int SourceIdCityWall = CitySceneConfig.SourceIdCityWall;
    public const int SourceIdCityRoof = CitySceneConfig.SourceIdCityRoof;

    public const float WorldBoundaryLeft = -960f;
    public const float WorldBoundaryRight = 1600f;
    public const float WorldBoundaryTop = -32f;
    public const float WorldBoundaryBottom = 1152f;

    public enum TransitionType
    {
        DoorToInterior,
        DoorToExterior,
        ExitToCity,
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

    public static readonly TileZone ExteriorRoom = new(1, 2, 17, 21);
    public static readonly TileZone InteriorRoom = new(22, 2, 17, 21);
    public static readonly TileZone Doorway = new(18, 12, 4, 4);
    public static readonly TileZone ExteriorApproach = new(4, 12, 14, 9);
    public static readonly TileZone InteriorNorthPath = new(27, 4, 6, 12);
    public static readonly TileZone InteriorSouthPath = new(32, 14, 7, 8);
    public static readonly TileZone TowerFacade = new(4, 4, 13, 10);
    public static readonly TileZone PortalCircle = new(29, 9, 4, 4);

    public static readonly GridPosition DefaultSpawnPosition = new(5, 20);
    public static readonly GridPosition CityEntrancePosition = new(5, 20);
    public static readonly GridPosition ExteriorEntryPosition = new(5, 20);
    public static readonly GridPosition ExteriorDoorTransitionPosition = new(18, 14);
    public static readonly GridPosition ExteriorDoorSpawnPosition = new(15, 14);
    public static readonly GridPosition InteriorDoorTransitionPosition = new(21, 14);
    public static readonly GridPosition InteriorEntryPosition = new(25, 14);
    public static readonly GridPosition CityExitTransitionPosition = new(37, 20);
    public static readonly GridPosition CityExitSpawnPosition = new(32, 20);
    public static readonly GridPosition MageAnchor = new(30, 7);

    public static readonly WorldPosition DefaultSpawnWorldPosition = GridToWorld(DefaultSpawnPosition);
    public static readonly WorldPosition CityEntranceWorldPosition = GridToWorld(CityEntrancePosition);
    public static readonly WorldPosition ExteriorEntryWorldPosition = GridToWorld(ExteriorEntryPosition);
    public static readonly WorldPosition ExteriorDoorTransitionWorldPosition = GridToWorld(ExteriorDoorTransitionPosition);
    public static readonly WorldPosition ExteriorDoorSpawnWorldPosition = GridToWorld(ExteriorDoorSpawnPosition);
    public static readonly WorldPosition InteriorDoorTransitionWorldPosition = GridToWorld(InteriorDoorTransitionPosition);
    public static readonly WorldPosition InteriorEntryWorldPosition = GridToWorld(InteriorEntryPosition);
    public static readonly WorldPosition CityExitTransitionWorldPosition = GridToWorld(CityExitTransitionPosition);
    public static readonly WorldPosition CityExitSpawnWorldPosition = GridToWorld(CityExitSpawnPosition);

    public static readonly PropPlacement[] Props =
    [
        new(PropRegistry.TowerWall.FileName, 5, 4, true),
        new(PropRegistry.TowerDoor.FileName, 17, 13, false),
        new(PropRegistry.PortalInactive.FileName, 30, 10, false),
        new(PropRegistry.Bookshelf.FileName, 24, 6, true),
        new(PropRegistry.Bookshelf.FileName, 35, 6, true),
        new(PropRegistry.Bookshelf.FileName, 24, 18, true),
        new(PropRegistry.AlchemyTable.FileName, 34, 15, true),
        new(PropRegistry.Fireplace.FileName, 24, 16, true),
    ];

    public static readonly TransitionEndpoint[] Transitions =
    [
        new(TransitionType.DoorToInterior, SceneResPath, "interior_door", ExteriorDoorTransitionPosition),
        new(TransitionType.DoorToExterior, SceneResPath, "exterior_door", InteriorDoorTransitionPosition),
        new(TransitionType.ExitToCity, MainMenuConfig.CityScenePath, "tower_exit", CityExitTransitionPosition),
    ];

    /// <summary>Converts grid coordinates to world-space pixels for the shared isometric TileSet.</summary>
    public static WorldPosition GridToWorld(GridPosition position) => GridToWorld(position.Col, position.Row);

    public static WorldPosition GridToWorld(float col, float row) =>
        new((col - row) * TileRegistry.TileWidth / 2f, (col + row) * TileRegistry.TileHeight / 2f);

    /// <summary>Returns the terrain tile for a Tower grid cell.</summary>
    public static string GetTileFileName(int col, int row)
    {
        if (PortalCircle.Contains(col, row) || Doorway.Contains(col, row)) return TileRegistry.CityStone.FileName;
        if (IsDividerWall(col, row) || TowerFacade.Contains(col, row)) return TileRegistry.CityWall.FileName;
        if (InteriorRoom.Contains(col, row)) return TileRegistry.CityStone.FileName;
        if (ExteriorRoom.Contains(col, row)) return TileRegistry.Cobblestone.FileName;
        return TileRegistry.CityWall.FileName;
    }

    /// <summary>Resolves a Tower tile to the shared TileSet source ID.</summary>
    public static int GetSourceId(string fileName) => fileName switch
    {
        "tile_cobble.png" => SourceIdCobblestone,
        "tile_city_stone.png" => SourceIdCityStone,
        "tile_city_wall.png" => SourceIdCityWall,
        "tile_city_roof.png" => SourceIdCityRoof,
        _ => throw new ArgumentOutOfRangeException(nameof(fileName), fileName, "Unknown Mage's Tower tile."),
    };

    /// <summary>Returns true if a tile or blocking prop prevents movement.</summary>
    public static bool IsWalkable(int col, int row)
    {
        var tileConfig = TileRegistry.GetByFileName(GetTileFileName(col, row));
        if (tileConfig?.IsWalkable != true)
            return false;

        return !Props.Any(prop => prop.IsBlocking && prop.Col == col && prop.Row == row);
    }

    private static bool IsDividerWall(int col, int row) =>
        col >= 18 && col < 22 && row >= 0 && row < GridRows && !Doorway.Contains(col, row);
}