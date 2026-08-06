using System;

namespace EchoForest.Core;

/// <summary>Represents a playable cutscene that signals when it has completed.</summary>
public interface ICutscene
{
    void Play(Action onComplete);
}