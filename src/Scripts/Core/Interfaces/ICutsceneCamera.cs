using System;

namespace EchoForest.Core;

/// <summary>Pans the game camera to a cutscene position.</summary>
public interface ICutsceneCamera
{
    void PanTo(CutscenePosition target, float duration, Action onComplete);
}