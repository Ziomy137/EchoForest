using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# typed event bus. One instance is created by the composition root and
/// injected into collaborating game systems; it is not a Godot autoload.
/// </summary>
public sealed class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlersByEventType = new();

    /// <inheritdoc/>
    public void Publish<TEvent>(TEvent gameEvent)
    {
        if (!_handlersByEventType.TryGetValue(typeof(TEvent), out var handlers))
            return;

        // Subscribers may safely unsubscribe while handling an event.
        var handlersSnapshot = handlers.ToArray();
        foreach (var handler in handlersSnapshot)
            ((Action<TEvent>)handler).Invoke(gameEvent);
    }

    /// <inheritdoc/>
    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (!_handlersByEventType.TryGetValue(typeof(TEvent), out var handlers))
        {
            handlers = [];
            _handlersByEventType.Add(typeof(TEvent), handlers);
        }

        handlers.Add(handler);
    }

    /// <inheritdoc/>
    public void Unsubscribe<TEvent>(Action<TEvent> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (!_handlersByEventType.TryGetValue(typeof(TEvent), out var handlers))
            return;

        handlers.Remove(handler);
        if (handlers.Count == 0)
            _handlersByEventType.Remove(typeof(TEvent));
    }

    /// <inheritdoc/>
    public void Clear() => _handlersByEventType.Clear();
}