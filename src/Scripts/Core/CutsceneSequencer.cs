using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# sequential cutscene player. It advances only when the active step
/// invokes its completion callback, allowing engine adapters to remain async.
/// </summary>
public sealed class CutsceneSequencer : ICutscene
{
    private readonly IReadOnlyList<ICutsceneStep> _steps;
    private readonly IInputHandler _input;
    private readonly IEventBus _eventBus;
    private readonly string _cutsceneId;
    private int _currentStepIndex;
    private Action? _onComplete;

    public CutsceneSequencer(
        IReadOnlyList<ICutsceneStep> steps,
        IInputHandler input,
        IEventBus? eventBus = null,
        string cutsceneId = "cutscene")
    {
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _eventBus = eventBus ?? new EventBus();
        _cutsceneId = string.IsNullOrWhiteSpace(cutsceneId)
            ? throw new ArgumentException("Cutscene ID must not be empty.", nameof(cutsceneId))
            : cutsceneId;
    }

    /// <summary>Whether the sequence is awaiting completion of a step.</summary>
    public bool IsPlaying { get; private set; }

    /// <inheritdoc/>
    public void Play(Action onComplete)
    {
        ArgumentNullException.ThrowIfNull(onComplete);
        if (IsPlaying)
            throw new InvalidOperationException("The cutscene is already playing.");

        IsPlaying = true;
        _currentStepIndex = 0;
        _onComplete = onComplete;
        _input.IsBlocked = true;
        _eventBus.Publish(new CutsceneStartedEvent(_cutsceneId));
        PlayNextStep();
    }

    private void PlayNextStep()
    {
        if (_currentStepIndex >= _steps.Count)
        {
            Finish();
            return;
        }

        var step = _steps[_currentStepIndex++];
        var completed = false;
        step.Play(() =>
        {
            if (completed || !IsPlaying)
                return;

            completed = true;
            PlayNextStep();
        });
    }

    private void Finish()
    {
        IsPlaying = false;
        _input.IsBlocked = false;
        _eventBus.Publish(new CutsceneEndedEvent(_cutsceneId));
        var onComplete = _onComplete;
        _onComplete = null;
        onComplete?.Invoke();
    }
}