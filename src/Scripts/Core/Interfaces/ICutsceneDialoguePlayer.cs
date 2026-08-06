using System;

namespace EchoForest.Core;

/// <summary>Plays a configured dialogue line for a cutscene.</summary>
public interface ICutsceneDialoguePlayer
{
    void PlayLine(string npcId, string lineId, Action onComplete);
}