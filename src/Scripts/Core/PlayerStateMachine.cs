using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>
/// Player-specific state machine with validated <see cref="PlayerState"/> transitions.
/// Valid transitions:
///   Idle     ↔ Walking
///   Walking  ↔ Running
///   Idle     → Running
///   Running  → Idle
///   Idle / Walking / Running → Jumping
///   Jumping  → Idle
///   Walking / Idle → Combat
///   Combat   → Idle
/// </summary>
public sealed class PlayerStateMachine : StateMachine<PlayerState>
{
    public PlayerStateMachine(PlayerState initialState = PlayerState.Idle)
        : base(initialState)
    {
        Initialize();
    }

    protected override HashSet<(PlayerState, PlayerState)>? BuildTransitions() =>
        new()
        {
            // Idle ↔ Walking
            (PlayerState.Idle,    PlayerState.Walking),
            (PlayerState.Walking, PlayerState.Idle),

            // Walking ↔ Running
            (PlayerState.Walking, PlayerState.Running),
            (PlayerState.Running, PlayerState.Walking),

            // Any movement → Jumping
            (PlayerState.Idle,    PlayerState.Jumping),
            (PlayerState.Walking, PlayerState.Jumping),
            (PlayerState.Running, PlayerState.Jumping),

            // Jumping → Idle
            (PlayerState.Jumping, PlayerState.Idle),

            // Enter/exit Combat
            (PlayerState.Idle,    PlayerState.Combat),
            (PlayerState.Walking, PlayerState.Combat),
            (PlayerState.Combat,  PlayerState.Idle),

            // Running → Idle (release all keys while sprinting)
            (PlayerState.Running, PlayerState.Idle),

            // Idle → Running (press Run from a standing start)
            (PlayerState.Idle, PlayerState.Running),
        };
}
