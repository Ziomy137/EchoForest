using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>Godot trigger that invokes an <see cref="AreaTransitionService"/> at an area boundary.</summary>
[ExcludeFromCodeCoverage(Justification = "Godot Area2D wrapper - requires scene tree")]
public partial class AreaTransitionNode : Area2D
{
    private const float FadeDuration = 0.25f;
    private bool _isTransitioning;
    private IAreaTransitionService? _transitionService;
    private IInputHandler? _input;
    private Func<PlayerControllerNode, SaveData>? _saveDataFactory;

    /// <summary>Target scene resource path configured by the owning area scene.</summary>
    [Export(PropertyHint.File, "*.tscn")]
    public string TargetArea { get; private set; } = string.Empty;

    /// <summary>Named entry marker to use in the target scene.</summary>
    [Export]
    public string SpawnPointId { get; private set; } = string.Empty;

    /// <summary>Visual behavior used while the target scene replaces the current one.</summary>
    [Export]
    public TransitionType TransitionType { get; private set; } = TransitionType.FadeToBlack;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public override void _ExitTree()
    {
        BodyEntered -= OnBodyEntered;
    }

    /// <summary>Configures a transition from a scene-owned path constant.</summary>
    public void Configure(
        string targetArea,
        string spawnPointId,
        TransitionType transitionType,
        IAreaTransitionService transitionService,
        IInputHandler input,
        Func<PlayerControllerNode, SaveData> saveDataFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetArea);
        ArgumentException.ThrowIfNullOrWhiteSpace(spawnPointId);
        ArgumentNullException.ThrowIfNull(transitionService);
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(saveDataFactory);
        TargetArea = targetArea;
        SpawnPointId = spawnPointId;
        TransitionType = transitionType;
        _transitionService = transitionService;
        _input = input;
        _saveDataFactory = saveDataFactory;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (_isTransitioning || body is not PlayerControllerNode player || string.IsNullOrEmpty(TargetArea))
            return;

        if (_transitionService is null || _input is null || _saveDataFactory is null)
            return;

        _isTransitioning = true;
        _input.IsBlocked = true;

        if (TransitionType == TransitionType.Instant)
        {
            CompleteTransition(player);
            return;
        }

        StartFade(player);
    }

    private void StartFade(PlayerControllerNode player)
    {
        var tree = GetTree();
        var overlay = new CanvasLayer { Layer = 1000 };
        var color = TransitionType == TransitionType.FadeToWhite ? Colors.White : Colors.Black;
        var fadeRect = new ColorRect
        {
            AnchorsPreset = (int)Control.LayoutPreset.FullRect,
            GrowHorizontal = Control.GrowDirection.Both,
            GrowVertical = Control.GrowDirection.Both,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Color = new Color(color, 0f),
        };

        overlay.AddChild(fadeRect);
        tree.Root.AddChild(overlay);

        var fadeOut = overlay.CreateTween();
        fadeOut.TweenProperty(fadeRect, "color", color, FadeDuration);
        fadeOut.Finished += () =>
        {
            CompleteTransition(player);
            tree.ProcessFrame += FadeIn;
        };

        void FadeIn()
        {
            tree.ProcessFrame -= FadeIn;
            if (!GodotObject.IsInstanceValid(overlay))
                return;

            var fadeIn = overlay.CreateTween();
            fadeIn.TweenProperty(fadeRect, "color", new Color(color, 0f), FadeDuration);
            fadeIn.Finished += overlay.QueueFree;
        }
    }

    private void CompleteTransition(PlayerControllerNode player)
    {
        var scenePath = GetTree().CurrentScene?.SceneFilePath ?? string.Empty;

        try
        {
            _transitionService!.TriggerTransition(scenePath, TargetArea, SpawnPointId, _saveDataFactory!(player));

            var changeResult = GetTree().ChangeSceneToFile(TargetArea);
            if (changeResult != Error.Ok)
            {
                _input!.IsBlocked = false;
                _isTransitioning = false;
            }
        }
        catch
        {
            _input!.IsBlocked = false;
            _isTransitioning = false;
            throw;
        }
    }
}