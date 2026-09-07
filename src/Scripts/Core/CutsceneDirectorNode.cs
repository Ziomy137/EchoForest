using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>
/// Godot adapter for cutscene presentation: fades, waits, camera pans, and
/// dialogue. The sequence logic itself remains in <see cref="CutsceneSequencer"/>.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper - requires scene tree")]
public partial class CutsceneDirectorNode : CanvasLayer, ICutsceneFader, ICutsceneTimer, ICutsceneCamera, ICutsceneDialoguePlayer
{
    private ColorRect _fadeOverlay = null!;
    private IsometricCameraNode _camera = null!;
    private DialogueBoxNode _dialogueBox = null!;
    private IEventBus _eventBus = null!;
    private IInputHandler _input = null!;
    private Tween? _fadeTween;

    public override void _Ready()
    {
        var cottage = GetTree().CurrentScene as CottageAreaNode
            ?? throw new InvalidOperationException("CutsceneDirectorNode requires CottageAreaNode as the current scene.");

        _fadeOverlay = GetNode<ColorRect>("FadeOverlay");
        _camera = cottage.GetNode<IsometricCameraNode>(CottageSceneConfig.CameraNodeName);
        _dialogueBox = cottage.GetNode<DialogueBoxNode>("DialogueBox");
        _eventBus = cottage.EventBus;
        _input = cottage.InputHandler;
        _fadeOverlay.Color = ToGodotColor(CutsceneColor.Clear);
    }

    public override void _ExitTree()
    {
        _fadeTween?.Kill();
    }

    /// <summary>Plays the first framework use case: the new-game mage attack flash.</summary>
    public void PlayIntroMageAttack(Action onComplete)
    {
        var steps = new List<ICutsceneStep>
        {
            new FadeStep(0f, CutsceneColor.Black, this),
            new WaitStep(0.25f, this),
            new PublishEventStep<MageAttackStartedEvent>(_eventBus, new MageAttackStartedEvent()),
            new FadeStep(0.08f, CutsceneColor.White, this),
            new FadeStep(0.6f, CutsceneColor.Black, this),
            new WaitStep(0.4f, this),
            new FadeStep(0.6f, CutsceneColor.Clear, this),
        };

        new CutsceneSequencer(steps, _input, _eventBus, "intro_mage_attack").Play(onComplete);
    }

    /// <inheritdoc/>
    public void FadeTo(CutsceneColor targetColor, float duration, Action onComplete)
    {
        ArgumentNullException.ThrowIfNull(onComplete);
        _fadeTween?.Kill();

        if (duration == 0f)
        {
            _fadeOverlay.Color = ToGodotColor(targetColor);
            onComplete();
            return;
        }

        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(_fadeOverlay, "color", ToGodotColor(targetColor), duration);
        _fadeTween.Finished += onComplete;
    }

    /// <inheritdoc/>
    public void Wait(float duration, Action onComplete)
    {
        ArgumentNullException.ThrowIfNull(onComplete);
        GetTree().CreateTimer(duration).Timeout += onComplete;
    }

    /// <inheritdoc/>
    public void PanTo(CutscenePosition target, float duration, Action onComplete)
    {
        _camera.PanTo(new Vector2(target.X, target.Y), duration, onComplete);
    }

    /// <inheritdoc/>
    public void PlayLine(string npcId, string lineId, Action onComplete)
    {
        _dialogueBox.PlayCutsceneLine(npcId, lineId, onComplete);
    }

    private static Color ToGodotColor(CutsceneColor color) => new(color.Red, color.Green, color.Blue, color.Alpha);
}