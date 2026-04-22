using System;
using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// Generic, Godot-decoupled state machine implementing <see cref="IStateMachine{TState}"/>.
/// Subclasses override <see cref="BuildTransitions"/> to define allowed transitions.
/// </summary>
public class StateMachine<TState> : IStateMachine<TState> where TState : struct, Enum
{
    private readonly Dictionary<TState, List<Action>> _enterCallbacks = new();
    private readonly Dictionary<TState, List<Action>> _exitCallbacks = new();

    // null means "all transitions allowed" (base class default)
    private HashSet<(TState From, TState To)>? _allowedTransitions;

    public TState CurrentState { get; private set; }

    public StateMachine(TState initialState)
    {
        CurrentState = initialState;
    }

    /// <summary>
    /// Called once after construction (by <see cref="Initialize"/>) to populate
    /// the allowed-transition set.  Override to return the set of allowed
    /// (from, to) pairs.  Return <c>null</c> to allow all transitions (default).
    /// </summary>
    protected virtual HashSet<(TState, TState)>? BuildTransitions() => null;

    /// <summary>
    /// Must be called at the end of every subclass constructor to load the
    /// transition table produced by <see cref="BuildTransitions"/>.  Calling
    /// <see cref="BuildTransitions"/> from here (rather than from the base-class
    /// constructor) avoids the C# virtual-member-call-in-constructor anti-pattern
    /// (CA2214): the subclass fields are fully initialised before this runs.
    /// </summary>
    protected void Initialize()
    {
        _allowedTransitions = BuildTransitions();
    }

    public void TransitionTo(TState newState)
    {
        if (!IsValidTransition(CurrentState, newState))
            throw new InvalidOperationException(
                $"Invalid transition: {CurrentState} → {newState}");

        InvokeCallbacks(_exitCallbacks, CurrentState);
        CurrentState = newState;
        InvokeCallbacks(_enterCallbacks, newState);
    }

    public void OnStateEnter(TState state, Action callback)
    {
        if (!_enterCallbacks.TryGetValue(state, out var list))
            _enterCallbacks[state] = list = new List<Action>();
        list.Add(callback);
    }

    public void OnStateExit(TState state, Action callback)
    {
        if (!_exitCallbacks.TryGetValue(state, out var list))
            _exitCallbacks[state] = list = new List<Action>();
        list.Add(callback);
    }

    public bool IsValidTransition(TState from, TState to)
    {
        if (_allowedTransitions is null) return true;
        return _allowedTransitions.Contains((from, to));
    }

    private static void InvokeCallbacks(Dictionary<TState, List<Action>> map, TState state)
    {
        if (map.TryGetValue(state, out var callbacks))
            foreach (var cb in callbacks)
                cb();
    }
}
