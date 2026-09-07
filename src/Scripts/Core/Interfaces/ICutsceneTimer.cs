using System;

namespace EchoForest.Core;

/// <summary>Schedules a cutscene continuation after a duration.</summary>
public interface ICutsceneTimer
{
    void Wait(float duration, Action onComplete);
}