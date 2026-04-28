using System;

namespace EchoForest.Core;

/// <summary>
/// Thrown by <see cref="SaveService"/> when a save file exists but cannot be
/// deserialised (corrupt JSON, missing required fields, version mismatch, etc.).
/// </summary>
public sealed class SaveDataException : Exception
{
    public SaveDataException(string message) : base(message) { }

    public SaveDataException(string message, Exception inner) : base(message, inner) { }
}
