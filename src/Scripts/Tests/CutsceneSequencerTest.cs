using System;
using System.Collections.Generic;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class CutsceneSequencerTest
{
    [Test]
    public void Play_PlaysStepsInOrder()
    {
        var order = new List<int>();
        var sequencer = new CutsceneSequencer(
        [
            new MockCutsceneStep(() => order.Add(1)),
            new MockCutsceneStep(() => order.Add(2)),
            new MockCutsceneStep(() => order.Add(3)),
        ],
        new MockInputHandler());

        sequencer.Play(() => { });

        Assert.That(order, Is.EqualTo([1, 2, 3]));
    }

    [Test]
    public void Play_BlocksInputUntilAsynchronousStepCompletes()
    {
        var input = new MockInputHandler();
        var step = new DeferredCutsceneStep();
        var sequencer = new CutsceneSequencer([step], input);

        sequencer.Play(() => { });

        Assert.Multiple(() =>
        {
            Assert.That(input.IsBlocked, Is.True);
            Assert.That(sequencer.IsPlaying, Is.True);
        });
    }

    [Test]
    public void Play_UnblocksInputAndInvokesCompletionAfterFinalStep()
    {
        var input = new MockInputHandler();
        var step = new DeferredCutsceneStep();
        var sequencer = new CutsceneSequencer([step], input);
        var completed = false;

        sequencer.Play(() => completed = true);
        step.Complete();

        Assert.Multiple(() =>
        {
            Assert.That(completed, Is.True);
            Assert.That(input.IsBlocked, Is.False);
            Assert.That(sequencer.IsPlaying, Is.False);
        });
    }

    [Test]
    public void Play_WithNoSteps_CompletesAndUnblocksInputImmediately()
    {
        var input = new MockInputHandler();
        var completed = false;
        var sequencer = new CutsceneSequencer([], input);

        sequencer.Play(() => completed = true);

        Assert.Multiple(() =>
        {
            Assert.That(completed, Is.True);
            Assert.That(input.IsBlocked, Is.False);
            Assert.That(sequencer.IsPlaying, Is.False);
        });
    }

    [Test]
    public void Play_PublishesStartedAndEndedEventsAroundSteps()
    {
        var bus = new EventBus();
        var events = new List<string>();
        bus.Subscribe<CutsceneStartedEvent>(gameEvent => events.Add($"start:{gameEvent.CutsceneId}"));
        bus.Subscribe<CutsceneEndedEvent>(gameEvent => events.Add($"end:{gameEvent.CutsceneId}"));
        var sequencer = new CutsceneSequencer([new MockCutsceneStep(() => events.Add("step"))], new MockInputHandler(), bus, "mage_attack");

        sequencer.Play(() => { });

        Assert.That(events, Is.EqualTo(["start:mage_attack", "step", "end:mage_attack"]));
    }

    [Test]
    public void PublishEventStep_PublishesEventBeforeCompleting()
    {
        var bus = new EventBus();
        string? startedQuestId = null;
        bus.Subscribe<QuestStartedEvent>(gameEvent => startedQuestId = gameEvent.QuestId);
        var step = new PublishEventStep<QuestStartedEvent>(bus, new QuestStartedEvent("q_kidnapped"));

        step.Play(() => { });

        Assert.That(startedQuestId, Is.EqualTo("q_kidnapped"));
    }

    [Test]
    public void FadeStep_DelegatesColorAndDuration()
    {
        var fader = new MockCutsceneFader();
        var step = new FadeStep(0.8f, CutsceneColor.White, fader);

        step.Play(() => { });

        Assert.Multiple(() =>
        {
            Assert.That(fader.TargetColor, Is.EqualTo(CutsceneColor.White));
            Assert.That(fader.Duration, Is.EqualTo(0.8f));
        });
    }

    [Test]
    public void WaitStep_DelegatesDuration()
    {
        var timer = new MockCutsceneTimer();
        var step = new WaitStep(1.2f, timer);

        step.Play(() => { });

        Assert.That(timer.Duration, Is.EqualTo(1.2f));
    }

    [Test]
    public void DialogueStep_DelegatesNpcAndLineIds()
    {
        var dialogue = new MockCutsceneDialoguePlayer();
        var step = new DialogueStep("wife", "intro_01", dialogue);

        step.Play(() => { });

        Assert.Multiple(() =>
        {
            Assert.That(dialogue.NpcId, Is.EqualTo("wife"));
            Assert.That(dialogue.LineId, Is.EqualTo("intro_01"));
        });
    }

    [Test]
    public void CameraPanStep_DelegatesTargetAndDuration()
    {
        var camera = new MockCutsceneCamera();
        var step = new CameraPanStep(new CutscenePosition(120f, 48f), 2f, camera);

        step.Play(() => { });

        Assert.Multiple(() =>
        {
            Assert.That(camera.Target, Is.EqualTo(new CutscenePosition(120f, 48f)));
            Assert.That(camera.Duration, Is.EqualTo(2f));
        });
    }

    private sealed class MockCutsceneStep(Action onPlay) : ICutsceneStep
    {
        public void Play(Action onComplete)
        {
            onPlay();
            onComplete();
        }
    }

    private sealed class DeferredCutsceneStep : ICutsceneStep
    {
        private Action? _onComplete;

        public void Play(Action onComplete) => _onComplete = onComplete;

        public void Complete() => _onComplete?.Invoke();
    }

    private sealed class MockCutsceneFader : ICutsceneFader
    {
        public CutsceneColor TargetColor { get; private set; }
        public float Duration { get; private set; }

        public void FadeTo(CutsceneColor targetColor, float duration, Action onComplete)
        {
            TargetColor = targetColor;
            Duration = duration;
            onComplete();
        }
    }

    private sealed class MockCutsceneTimer : ICutsceneTimer
    {
        public float Duration { get; private set; }

        public void Wait(float duration, Action onComplete)
        {
            Duration = duration;
            onComplete();
        }
    }

    private sealed class MockCutsceneDialoguePlayer : ICutsceneDialoguePlayer
    {
        public string? NpcId { get; private set; }
        public string? LineId { get; private set; }

        public void PlayLine(string npcId, string lineId, Action onComplete)
        {
            NpcId = npcId;
            LineId = lineId;
            onComplete();
        }
    }

    private sealed class MockCutsceneCamera : ICutsceneCamera
    {
        public CutscenePosition Target { get; private set; }
        public float Duration { get; private set; }

        public void PanTo(CutscenePosition target, float duration, Action onComplete)
        {
            Target = target;
            Duration = duration;
            onComplete();
        }
    }
}