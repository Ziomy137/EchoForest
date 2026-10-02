using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>Pure-C# layout data for the 60 by 60 City hub.</summary>
public static class CitySceneConfig
{
    public const int GridColumns = 60;
    public const int GridRows = 60;
    public const int TotalCells = GridColumns * GridRows;

    public const string SceneResPath = MainMenuConfig.CityScenePath;
    public const string TileSetResPath = CottageSceneConfig.TileSetResPath;
    public const string TileMapLayerName = "TileMapLayer";
    public const string BoundaryNodeName = "Boundary";
    public const string CameraNodeName = "Camera";
    public const string PlayerSpawnName = "PlayerSpawnPoint";
    public const string MarketDistrictNodeName = "MarketDistrict";

    public const int SourceIdWater = 5;
    public const int SourceIdCobblestone = 20;
    public const int SourceIdCobblestoneVariation = 21;
    public const int SourceIdCityStone = 22;
    public const int SourceIdCityWall = 23;
    public const int SourceIdCityRoof = 24;
    public const int SourceIdStallTop = 25;

    public const float WorldBoundaryLeft = -1920f;
    public const float WorldBoundaryRight = 1920f;
    public const float WorldBoundaryTop = -32f;
    public const float WorldBoundaryBottom = 1952f;

    /// <summary>Axis-aligned rectangle of cells in the city grid.</summary>
    public readonly record struct TileZone(int Col, int Row, int Width, int Height)
    {
        public int TileCount => Width * Height;
        public bool Contains(int col, int row) =>
            col >= Col && col < Col + Width && row >= Row && row < Row + Height;
    }

    /// <summary>Grid coordinate of a city landmark or marker.</summary>
    public readonly record struct GridPosition(int Col, int Row);

    /// <summary>Engine-neutral world-space coordinate.</summary>
    public readonly record struct WorldPosition(float X, float Y);

    /// <summary>City prop placement, optionally resolved from a market sub-scene marker.</summary>
    public readonly record struct PropPlacement(string FileName, int Col, int Row, bool IsBlocking, string? MarkerName = null);

    /// <summary>Named anchor for an NPC that will be added in Sprint 14.</summary>
    public readonly record struct NpcAnchor(string Name, int Col, int Row);

    public static readonly TileZone MainStreet = new(28, 0, 4, GridRows);
    public static readonly TileZone MarketCrossStreet = new(4, 34, 52, 3);
    public static readonly TileZone TowerAccessStreet = new(30, 16, 17, 4);
    public static readonly TileZone MarketPlaza = new(12, 29, 40, 14);

    public static readonly TileZone[] MarketStallPads =
    [
        new(13, 31, 3, 2),
        new(19, 38, 3, 2),
        new(37, 31, 3, 2),
        new(43, 38, 3, 2),
    ];

    public static readonly TileZone[] BuildingFootprints =
    [
        new(5, 16, 11, 11),
        new(7, 43, 12, 12),
        new(42, 21, 12, 11),
        new(42, 43, 12, 11),
        new(37, 2, 16, 14),
    ];

    public static readonly GridPosition SouthTransitionGridPosition = new(30, 58);
    public static readonly GridPosition SouthEntranceSpawnGridPosition = new(30, 53);
    public static readonly GridPosition DefaultSpawnPosition = SouthEntranceSpawnGridPosition;
    public static readonly GridPosition TowerTransitionGridPosition = new(44, 17);
    public static readonly GridPosition TowerExitSpawnGridPosition = new(44, 20);
    public static readonly GridPosition CityGateGridPosition = new(30, 57);
    public static readonly GridPosition TowerDoorGridPosition = new(44, 16);

    public static readonly WorldPosition SouthTransitionPosition = GridToWorld(SouthTransitionGridPosition);
    public static readonly WorldPosition SouthEntranceSpawnPosition = GridToWorld(SouthEntranceSpawnGridPosition);
    public static readonly WorldPosition TowerTransitionPosition = GridToWorld(TowerTransitionGridPosition);
    public static readonly WorldPosition TowerExitSpawnPosition = GridToWorld(TowerExitSpawnGridPosition);
    public static readonly WorldPosition TowerDoorPosition = GridToWorld(TowerDoorGridPosition);

    public static readonly PropPlacement[] Props =
    [
        new(PropRegistry.LampPost.FileName, 24, 25, false),
        new(PropRegistry.LampPost.FileName, 35, 25, false),
        new(PropRegistry.LampPost.FileName, 24, 47, false),
        new(PropRegistry.LampPost.FileName, 35, 47, false),
        new(PropRegistry.LampPost.FileName, 24, 18, false),
        new(PropRegistry.LampPost.FileName, 35, 18, false),
        new(PropRegistry.Fountain.FileName, 34, 36, true, "FountainCenter"),
        new(PropRegistry.MarketStall.FileName, 14, 32, true, "StallMarkerOne"),
        new(PropRegistry.MarketStall.FileName, 20, 40, true, "StallMarkerTwo"),
        new(PropRegistry.MarketStall.FileName, 40, 32, true, "StallMarkerThree"),
        new(PropRegistry.MarketStall.FileName, 46, 40, true, "StallMarkerFour"),
        new(PropRegistry.NoticeBoard.FileName, 32, 52, false),
        new(PropRegistry.CityGate.FileName, 30, 57, false),
        new(PropRegistry.TowerWall.FileName, 44, 7, true),
        new(PropRegistry.TowerDoor.FileName, 44, 16, true),
    ];

    public static readonly NpcAnchor[] NpcAnchors =
    [
        new("MerchantAnchor", 18, 35),
        new("CityGuardAnchor", 30, 53),
        new("TownspersonAnchorOne", 17, 43),
        new("TownspersonAnchorTwo", 39, 42),
        new("TownspersonAnchorThree", 34, 28),
    ];

    public static readonly GridPosition[] GuardPostPositions =
    [
        new(27, 54),
        new(32, 35),
    ];

    /// <summary>Converts isometric grid coordinates into world-space pixels.</summary>
    public static WorldPosition GridToWorld(GridPosition position) =>
        GridToWorld(position.Col, position.Row);

    /// <summary>Converts fractional isometric grid coordinates into world-space pixels.</summary>
    public static WorldPosition GridToWorld(float col, float row) =>
        new((col - row) * TileRegistry.TileWidth / 2f, (col + row) * TileRegistry.TileHeight / 2f);

    /// <summary>Returns the first market/building/street tile matching a grid cell.</summary>
    public static string GetTileFileName(int col, int row)
    {
        foreach (var building in BuildingFootprints)
        {
            if (!building.Contains(col, row))
                continue;

            var roofRows = Math.Max(1, building.Height / 2);
            return row < building.Row + roofRows
                ? TileRegistry.CityRoof.FileName
                : TileRegistry.CityWall.FileName;
        }

        foreach (var stallPad in MarketStallPads)
        {
            if (stallPad.Contains(col, row))
                return TileRegistry.StallTop.FileName;
        }

        if (MarketPlaza.Contains(col, row))
            return TileRegistry.CityStone.FileName;

        if (MainStreet.Contains(col, row) || MarketCrossStreet.Contains(col, row) || TowerAccessStreet.Contains(col, row))
            return GetCobblestoneVariation(col, row);

        return GetCobblestoneVariation(col, row);
    }

    /// <summary>Resolves a city tile filename to its stable shared TileSet source ID.</summary>
    public static int GetSourceId(string fileName) => fileName switch
    {
        "tile_water.png" => SourceIdWater,
        "tile_cobble.png" => SourceIdCobblestone,
        "tile_cobble_var.png" => SourceIdCobblestoneVariation,
        "tile_city_stone.png" => SourceIdCityStone,
        "tile_city_wall.png" => SourceIdCityWall,
        "tile_city_roof.png" => SourceIdCityRoof,
        "tile_stall_top.png" => SourceIdStallTop,
        _ => throw new ArgumentOutOfRangeException(nameof(fileName), fileName, "Unknown City tile."),
    };

    private static string GetCobblestoneVariation(int col, int row) =>
        (col * 7 + row * 11) % 17 == 0
            ? TileRegistry.CobblestoneVariation.FileName
            : TileRegistry.Cobblestone.FileName;
}