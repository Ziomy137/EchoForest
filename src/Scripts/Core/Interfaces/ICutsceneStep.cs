using System;

namespace EchoForest.Core;

/// <summary>One asynchronous operation in a cutscene sequence.</summary>
public interface ICutsceneStep
{
    void Play(Action onComplete);
}