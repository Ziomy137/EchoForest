using System;

namespace EchoForest.Core;

/// <summary>Raised when a quest data file cannot be read or fails validation.</summary>
public sealed class QuestLoadException : Exception
{
    public QuestLoadException(string message)
        : base(message)
    {
    }

    public QuestLoadException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}