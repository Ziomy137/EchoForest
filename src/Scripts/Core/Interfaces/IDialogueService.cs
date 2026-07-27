namespace EchoForest.Core;

/// <summary>Loads dialogue trees and resolves their line links.</summary>
public interface IDialogueService
{
    /// <summary>Loads and makes active the dialogue tree for <paramref name="npcId"/>.</summary>
    DialogueTree LoadDialogue(string npcId);

    /// <summary>Gets a line from the active dialogue tree.</summary>
    DialogueLine GetLine(string lineId);

    /// <summary>Gets the line linked after <paramref name="currentLineId"/>, or <c>null</c> at the end.</summary>
    DialogueLine? GetNextLine(string currentLineId);
}