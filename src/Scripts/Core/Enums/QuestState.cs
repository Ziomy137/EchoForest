namespace EchoForest.Core;

/// <summary>
/// Tracks the progress state of a single quest.
/// </summary>
public enum QuestState
{
    /// <summary>Quest not yet started.</summary>
    NotStarted,

    /// <summary>Quest is currently active.</summary>
    InProgress,

    /// <summary>Quest has been successfully completed.</summary>
    Completed,

    /// <summary>Quest was failed or abandoned.</summary>
    Failed,
}
