using System;

namespace EchoForest.Core;

/// <summary>Thrown when an NPC dialogue file cannot be read or validated.</summary>
public sealed class DialogueLoadException : Exception
{
    public DialogueLoadException(string message) : base(message) { }

    public DialogueLoadException(string message, Exception innerException) : base(message, innerException) { }
}