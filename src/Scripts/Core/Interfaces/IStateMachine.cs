using System;

namespace EchoForest.Core;

/// <summary>
/// Generic state machine contract. Fully decoupled from Godot — testable with NUnit.
/// </summary>
/// <typeparam name="TState">An enum type representing all possible states.</typeparam>
public interface IStateMachine<TState> where TState : struct, Enum
{
    /// <summary>The currently active state.</summary>
    TState CurrentState { get; }

    /// <summary>
    /// Transitions to <paramref name="newState"/>.
    /// Throws <see cref="InvalidOperationException"/> if the transition is not allowed.
    /// </summary>
    void TransitionTo(TState newState);

    /// <summary>Registers a callback invoked when entering <paramref name="state"/>.</summary>
    void OnStateEnter(TState state, Action callback);

    /// <summary>Registers a callback invoked when exiting <paramref name="state"/>.</summary>
    void OnStateExit(TState state, Action callback);

    /// <summary>Returns whether a direct transition from <paramref name="from"/> to <paramref name="to"/> is valid.</summary>
    bool IsValidTransition(TState from, TState to);
}
