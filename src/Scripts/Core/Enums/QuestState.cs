using System;

namespace EchoForest.Core;

/// <summary>
/// Tracks the progress state of a single quest.
/// </summary>
public enum QuestState
{
    /// <summary>Quest not yet started.</summary>
    NotStarted,

    /// <summary>Quest is currently active.</summary>
    Active,

    /// <summary>Legacy serialized name for <see cref="Active"/>.</summary>
    [Obsolete("Use Active instead.")]
    InProgress = Active,

    /// <summary>Quest has been successfully completed.</summary>
    Completed,

    /// <summary>Quest was failed or abandoned.</summary>
    Failed,
}
