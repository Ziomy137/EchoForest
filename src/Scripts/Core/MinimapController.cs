using System;
using System.Linq;

namespace EchoForest.Core;

/// <summary>Pixel-space position of the player marker inside the 64 by 64 minimap.</summary>
public readonly record struct MinimapPoint(float X, float Y);

/// <summary>Pure-C# state and world-to-map transform for the shared game minimap.</summary>
public sealed class MinimapController : IDisposable
{
    public const int MapWidth = 64;
    public const int MapHeight = 64;
    public const int IsometricMapTop = 16;
    public const int IsometricMapHeight = 32;

    private const string TextureDirectory = "res://src/Assets/Sprites/Minimaps";

    private static readonly MinimapArea[] Areas =
    [
        new("cottage", "Cottage", CottageSceneConfig.SceneResPath,
            $"{TextureDirectory}/minimap_cottage.png",
            CottageSceneConfig.WorldBoundaryLeft, CottageSceneConfig.WorldBoundaryTop,
            CottageSceneConfig.WorldBoundaryRight, CottageSceneConfig.WorldBoundaryBottom),
        new("farm", "Farm", FarmSceneConfig.SceneResPath,
            $"{TextureDirectory}/minimap_farm.png",
            FarmSceneConfig.WorldBoundaryLeft, FarmSceneConfig.WorldBoundaryTop,
            FarmSceneConfig.WorldBoundaryRight, FarmSceneConfig.WorldBoundaryBottom),
        new("forest_path", "Forest Path", ForestPathSceneConfig.SceneResPath,
            $"{TextureDirectory}/minimap_forest_path.png",
            ForestPathSceneConfig.WorldBoundaryLeft, ForestPathSceneConfig.WorldBoundaryTop,
            ForestPathSceneConfig.WorldBoundaryRight, ForestPathSceneConfig.WorldBoundaryBottom),
        new("city", "City", CitySceneConfig.SceneResPath,
            $"{TextureDirectory}/minimap_city.png",
            CitySceneConfig.WorldBoundaryLeft, CitySceneConfig.WorldBoundaryTop,
            CitySceneConfig.WorldBoundaryRight, CitySceneConfig.WorldBoundaryBottom),
        new("mages_tower", "Mage's Tower", MagesTowerSceneConfig.SceneResPath,
            $"{TextureDirectory}/minimap_mages_tower.png",
            MagesTowerSceneConfig.WorldBoundaryLeft, MagesTowerSceneConfig.WorldBoundaryTop,
            MagesTowerSceneConfig.WorldBoundaryRight, MagesTowerSceneConfig.WorldBoundaryBottom),
        new("portal_chamber", "Portal Chamber", PortalChamberSceneConfig.SceneResPath,
            $"{TextureDirectory}/minimap_portal_chamber.png",
            PortalChamberSceneConfig.WorldBoundaryLeft, PortalChamberSceneConfig.WorldBoundaryTop,
            PortalChamberSceneConfig.WorldBoundaryRight, PortalChamberSceneConfig.WorldBoundaryBottom),
    ];

    private readonly IEventBus _eventBus;
    private MinimapArea _currentArea;
    private bool _disposed;

    public MinimapController(IEventBus eventBus, string initialScenePath = "")
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _currentArea = Areas[0];
        SetArea(string.IsNullOrWhiteSpace(initialScenePath) ? CottageSceneConfig.SceneResPath : initialScenePath);
        _eventBus.Subscribe<AreaTransitionEvent>(OnAreaTransition);
        PlayerDotPosition = new MinimapPoint(MapWidth / 2f, MapHeight / 2f);
    }

    public string CurrentAreaId => _currentArea.Id;
    public string CurrentAreaName => _currentArea.Name;
    public string CurrentMapTexturePath => _currentArea.TexturePath;
    public bool IsVisible { get; private set; } = true;
    public MinimapPoint PlayerDotPosition { get; private set; }

    /// <summary>Maps world coordinates to pixel coordinates, clamped to the visible map.</summary>
    public void UpdatePlayerPosition(float worldX, float worldY)
    {
        var mapX = (worldX - _currentArea.Left) / _currentArea.Width * MapWidth;
        var mapY = IsometricMapTop + (worldY - _currentArea.Top) / _currentArea.Height * IsometricMapHeight;
        PlayerDotPosition = new MinimapPoint(
            Math.Clamp(mapX, 0f, MapWidth - 1f),
            Math.Clamp(mapY, IsometricMapTop, IsometricMapTop + IsometricMapHeight - 1f));
    }

    public void ToggleVisibility() => IsVisible = !IsVisible;

    public void Dispose()
    {
        if (_disposed)
            return;

        _eventBus.Unsubscribe<AreaTransitionEvent>(OnAreaTransition);
        _disposed = true;
    }

    private void OnAreaTransition(AreaTransitionEvent gameEvent) => SetArea(gameEvent.ToArea);

    private void SetArea(string scenePath)
    {
        _currentArea = Areas.SingleOrDefault(area => area.ScenePath == scenePath)
            ?? throw new ArgumentException($"No minimap is configured for scene '{scenePath}'.", nameof(scenePath));
    }

    private sealed record MinimapArea(
        string Id,
        string Name,
        string ScenePath,
        string TexturePath,
        float Left,
        float Top,
        float Right,
        float Bottom)
    {
        public float Width => Right - Left;
        public float Height => Bottom - Top;
    }
}