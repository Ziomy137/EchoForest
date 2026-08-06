using System;

namespace EchoForest.Core;

/// <summary>Controls the state and events of one active NPC conversation.</summary>
public interface IDialogueController
{
    bool IsConversationActive { get; }
    DialogueLine? CurrentLine { get; }

    event Action? OnConversationStarted;
    event Action<DialogueLine>? OnLineChanged;
    event Action? OnConversationEnded;

    void StartConversation(string npcId);
    void StartConversationAtLine(string npcId, string lineId);
    bool Advance();
    void EndConversation();
}