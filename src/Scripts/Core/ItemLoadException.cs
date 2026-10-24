using System;

namespace EchoForest.Core;

/// <summary>Raised when the item catalog cannot be read or contains invalid definitions.</summary>
public sealed class ItemLoadException : Exception
{
    public ItemLoadException(string message)
        : base(message)
    {
    }

    public ItemLoadException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}