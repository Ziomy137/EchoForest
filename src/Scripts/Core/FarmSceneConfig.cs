namespace EchoForest.Core;

/// <summary>Pure-C# layout data for the 40 by 25 Farm area.</summary>
public static class FarmSceneConfig
{
    public const int GridColumns = 40;
    public const int GridRows = 25;
    public const int TotalCells = GridColumns * GridRows;

    public const string SceneResPath = MainMenuConfig.FarmScenePath;
    public const string TileSetResPath = CottageSceneConfig.TileSetResPath;
    public const string TileMapLayerName = "TileMapLayer";
    public const string BoundaryNodeName = "Boundary";
    public const string CameraNodeName = "Camera";
    public const string PlayerSpawnName = "PlayerSpawnPoint";

    public const int SourceIdGrass = 0;
    public const int SourceIdDirt = 2;
    public const int SourceIdCropYoung = 11;
    public const int SourceIdCropMature = 12;
    public const int SourceIdSoilDry = 13;
    public const int SourceIdIrrigation = 14;
    public const int SourceIdHay = 15;

    public const float WorldBoundaryLeft = -800f;
    public const float WorldBoundaryRight = 1280f;
    public const float WorldBoundaryTop = -32f;
    public const float WorldBoundaryBottom = 1040f;

    /// <summary>Axis-aligned rectangle of cells within the Farm grid.</summary>
    public readonly record struct TileZone(int Col, int Row, int Width, int Height)
    {
        public int TileCount => Width * Height;
        public bool Contains(int col, int row) =>
            col >= Col && col < Col + Width && row >= Row && row < Row + Height;
    }

    /// <summary>File name and grid location for a Farm prop.</summary>
    public readonly record struct PropPlacement(string FileName, int Col, int Row, bool IsBlocking);

    public static readonly TileZone MatureCropField = new(5, 11, 6, 4);
    public static readonly TileZone YoungCropField = new(28, 11, 6, 4);
    public static readonly TileZone DrySoilField = new(5, 16, 6, 4);
    public static readonly TileZone HayField = new(28, 16, 6, 4);
    public static readonly TileZone IrrigationDitch = new(2, 8, 36, 1);
    public static readonly TileZone NorthSouthPath = new(18, 0, 3, 25);
    public static readonly TileZone WestPath = new(0, 20, 18, 3);

    public static readonly PropPlacement[] Props =
    {
        new(PropRegistry.Barn.FileName, 32, 3, true),
        new(PropRegistry.Scarecrow.FileName, 17, 13, true),
        new(PropRegistry.Plow.FileName, 29, 6, true),
    };

    /// <summary>Returns the highest-priority terrain tile for one Farm cell.</summary>
    public static string GetTileFileName(int col, int row)
    {
        if (MatureCropField.Contains(col, row)) return TileRegistry.CropMature.FileName;
        if (YoungCropField.Contains(col, row)) return TileRegistry.CropYoung.FileName;
        if (DrySoilField.Contains(col, row)) return TileRegistry.SoilDry.FileName;
        if (HayField.Contains(col, row)) return TileRegistry.Hay.FileName;
        if (IrrigationDitch.Contains(col, row)) return TileRegistry.Irrigation.FileName;
        if (NorthSouthPath.Contains(col, row) || WestPath.Contains(col, row)) return TileRegistry.Dirt.FileName;
        return TileRegistry.Grass.FileName;
    }

    /// <summary>Resolves a configured Farm tile to its shared TileSet source ID.</summary>
    public static int GetSourceId(string fileName) => fileName switch
    {
        "tile_grass.png" => SourceIdGrass,
        "tile_dirt.png" => SourceIdDirt,
        "tile_crop_young.png" => SourceIdCropYoung,
        "tile_crop_mature.png" => SourceIdCropMature,
        "tile_soil_dry.png" => SourceIdSoilDry,
        "tile_irrigation.png" => SourceIdIrrigation,
        "tile_hay.png" => SourceIdHay,
        _ => throw new System.ArgumentOutOfRangeException(nameof(fileName), fileName, "Unknown Farm tile."),
    };
}