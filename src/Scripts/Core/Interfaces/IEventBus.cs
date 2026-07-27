using System;

namespace EchoForest.Core;

/// <summary>
/// In-process, typed event channel shared by game systems through dependency
/// injection. It has no Godot runtime dependency.
/// </summary>
public interface IEventBus
{
    void Publish<TEvent>(TEvent gameEvent);
    void Subscribe<TEvent>(Action<TEvent> handler);
    void Unsubscribe<TEvent>(Action<TEvent> handler);
    void Clear();
}