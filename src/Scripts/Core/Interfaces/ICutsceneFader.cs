using System;

namespace EchoForest.Core;

/// <summary>Renders a screen fade for a cutscene step.</summary>
public interface ICutsceneFader
{
    void FadeTo(CutsceneColor targetColor, float duration, Action onComplete);
}