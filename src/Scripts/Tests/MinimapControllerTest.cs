using System.IO;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class MinimapControllerTest
{
    [Test]
    public void Constructor_UsesCottageMapByDefault()
    {
        var controller = new MinimapController(new EventBus());

        Assert.Multiple(() =>
        {
            Assert.That(controller.CurrentAreaId, Is.EqualTo("cottage"));
            Assert.That(controller.CurrentAreaName, Is.EqualTo("Cottage"));
            Assert.That(controller.CurrentMapTexturePath, Does.EndWith("minimap_cottage.png"));
            Assert.That(controller.IsVisible, Is.True);
        });
    }

    [Test]
    public void UpdatePlayerPosition_MapsWorldCenterToMapCenter()
    {
        var controller = new MinimapController(new EventBus(), MainMenuConfig.CityScenePath);
        var cityCenterX = (CitySceneConfig.WorldBoundaryLeft + CitySceneConfig.WorldBoundaryRight) / 2f;
        var cityCenterY = (CitySceneConfig.WorldBoundaryTop + CitySceneConfig.WorldBoundaryBottom) / 2f;

        controller.UpdatePlayerPosition(cityCenterX, cityCenterY);

        Assert.That(controller.PlayerDotPosition, Is.EqualTo(new MinimapPoint(32f, 32f)));
    }

    [Test]
    public void UpdatePlayerPosition_ClampsToMapEdges()
    {
        var controller = new MinimapController(new EventBus(), MainMenuConfig.FarmScenePath);
        controller.UpdatePlayerPosition(FarmSceneConfig.WorldBoundaryRight + 100f, FarmSceneConfig.WorldBoundaryTop - 100f);

        Assert.That(controller.PlayerDotPosition, Is.EqualTo(new MinimapPoint(63f, MinimapController.IsometricMapTop)));
    }

    [Test]
    public void UpdatePlayerPosition_MapsWorldVerticalBoundsToIsometricBand()
    {
        var controller = new MinimapController(new EventBus(), FarmSceneConfig.SceneResPath);
        controller.UpdatePlayerPosition(0f, FarmSceneConfig.WorldBoundaryTop);
        Assert.That(controller.PlayerDotPosition.Y, Is.EqualTo(MinimapController.IsometricMapTop));

        controller.UpdatePlayerPosition(0f, FarmSceneConfig.WorldBoundaryBottom);
        Assert.That(controller.PlayerDotPosition.Y, Is.EqualTo(
            MinimapController.IsometricMapTop + MinimapController.IsometricMapHeight - 1f));
    }

    [Test]
    public void AreaTransition_UpdatesMapAndAreaName()
    {
        var eventBus = new EventBus();
        var controller = new MinimapController(eventBus, MainMenuConfig.CityScenePath);

        eventBus.Publish(new AreaTransitionEvent(MainMenuConfig.CityScenePath, MainMenuConfig.PortalChamberScenePath));

        Assert.Multiple(() =>
        {
            Assert.That(controller.CurrentAreaId, Is.EqualTo("portal_chamber"));
            Assert.That(controller.CurrentAreaName, Is.EqualTo("Portal Chamber"));
            Assert.That(controller.CurrentMapTexturePath, Does.EndWith("minimap_portal_chamber.png"));
        });
    }

    [Test]
    public void AllAreas_HaveAnExistingMapTexture()
    {
        var repositoryRoot = FindRepositoryRoot();
        var scenePaths = new[]
        {
            CottageSceneConfig.SceneResPath,
            FarmSceneConfig.SceneResPath,
            ForestPathSceneConfig.SceneResPath,
            CitySceneConfig.SceneResPath,
            MagesTowerSceneConfig.SceneResPath,
            PortalChamberSceneConfig.SceneResPath,
        };

        foreach (var scenePath in scenePaths)
        {
            var controller = new MinimapController(new EventBus(), scenePath);
            var mapFilePath = Path.Combine(repositoryRoot,
                controller.CurrentMapTexturePath["res://".Length..].Replace('/', Path.DirectorySeparatorChar));
            Assert.That(File.Exists(mapFilePath), Is.True, $"Missing minimap texture for {scenePath}");
        }
    }

    [Test]
    public void ToggleVisibility_TogglesMapVisibility()
    {
        var controller = new MinimapController(new EventBus());

        controller.ToggleVisibility();
        Assert.That(controller.IsVisible, Is.False);

        controller.ToggleVisibility();
        Assert.That(controller.IsVisible, Is.True);
    }

    [Test]
    public void Dispose_UnsubscribesFromAreaTransitions()
    {
        var eventBus = new EventBus();
        var controller = new MinimapController(eventBus);
        controller.Dispose();

        eventBus.Publish(new AreaTransitionEvent(MainMenuConfig.ContinueScenePath, MainMenuConfig.FarmScenePath));

        Assert.That(controller.CurrentAreaId, Is.EqualTo("cottage"));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "EchoForest.sln")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the repository root from the NUnit test directory.");
    }
}