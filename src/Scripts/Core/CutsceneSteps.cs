using System;

namespace EchoForest.Core;

/// <summary>Fades the presentation to a color through an injected renderer.</summary>
public sealed class FadeStep : ICutsceneStep
{
    private readonly float _duration;
    private readonly CutsceneColor _targetColor;
    private readonly ICutsceneFader _fader;

    public FadeStep(float duration, CutsceneColor targetColor, ICutsceneFader fader)
    {
        if (duration < 0f)
            throw new ArgumentOutOfRangeException(nameof(duration));

        _duration = duration;
        _targetColor = targetColor;
        _fader = fader ?? throw new ArgumentNullException(nameof(fader));
    }

    public void Play(Action onComplete)
    {
        ArgumentNullException.ThrowIfNull(onComplete);
        _fader.FadeTo(_targetColor, _duration, onComplete);
    }
}

/// <summary>Waits through an injected scheduler.</summary>
public sealed class WaitStep : ICutsceneStep
{
    private readonly float _duration;
    private readonly ICutsceneTimer _timer;

    public WaitStep(float duration, ICutsceneTimer timer)
    {
        if (duration < 0f)
            throw new ArgumentOutOfRangeException(nameof(duration));

        _duration = duration;
        _timer = timer ?? throw new ArgumentNullException(nameof(timer));
    }

    public void Play(Action onComplete)
    {
        ArgumentNullException.ThrowIfNull(onComplete);
        _timer.Wait(_duration, onComplete);
    }
}

/// <summary>Shows one named NPC dialogue line through an injected presenter.</summary>
public sealed class DialogueStep : ICutsceneStep
{
    private readonly string _npcId;
    private readonly string _lineId;
    private readonly ICutsceneDialoguePlayer _dialoguePlayer;

    public DialogueStep(string npcId, string lineId, ICutsceneDialoguePlayer dialoguePlayer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(npcId);
        ArgumentException.ThrowIfNullOrWhiteSpace(lineId);
        _npcId = npcId;
        _lineId = lineId;
        _dialoguePlayer = dialoguePlayer ?? throw new ArgumentNullException(nameof(dialoguePlayer));
    }

    public void Play(Action onComplete)
    {
        ArgumentNullException.ThrowIfNull(onComplete);
        _dialoguePlayer.PlayLine(_npcId, _lineId, onComplete);
    }
}

/// <summary>Pans through an injected camera adapter.</summary>
public sealed class CameraPanStep : ICutsceneStep
{
    private readonly CutscenePosition _target;
    private readonly float _duration;
    private readonly ICutsceneCamera _camera;

    public CameraPanStep(CutscenePosition target, float duration, ICutsceneCamera camera)
    {
        if (duration < 0f)
            throw new ArgumentOutOfRangeException(nameof(duration));

        _target = target;
        _duration = duration;
        _camera = camera ?? throw new ArgumentNullException(nameof(camera));
    }

    public void Play(Action onComplete)
    {
        ArgumentNullException.ThrowIfNull(onComplete);
        _camera.PanTo(_target, _duration, onComplete);
    }
}

/// <summary>Publishes an event at a defined point in a cutscene sequence.</summary>
public sealed class PublishEventStep<TEvent> : ICutsceneStep
{
    private readonly IEventBus _eventBus;
    private readonly TEvent _gameEvent;

    public PublishEventStep(IEventBus eventBus, TEvent gameEvent)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _gameEvent = gameEvent;
    }

    public void Play(Action onComplete)
    {
        ArgumentNullException.ThrowIfNull(onComplete);
        _eventBus.Publish(_gameEvent);
        onComplete();
    }
}