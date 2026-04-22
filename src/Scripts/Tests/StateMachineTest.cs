using System;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class StateMachineTest
{
    [Test]
    public void StateMachine_InitialState_IsSetCorrectly()
    {
        var sm = new StateMachine<PlayerState>(PlayerState.Idle);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Idle));
    }

    [Test]
    public void TransitionTo_ValidState_UpdatesCurrentState()
    {
        var sm = new StateMachine<PlayerState>(PlayerState.Idle);
        sm.TransitionTo(PlayerState.Walking);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Walking));
    }

    [Test]
    public void OnStateEnter_IsCalled_WhenEnteringState()
    {
        bool called = false;
        var sm = new StateMachine<PlayerState>(PlayerState.Idle);
        sm.OnStateEnter(PlayerState.Walking, () => called = true);
        sm.TransitionTo(PlayerState.Walking);
        Assert.That(called, Is.True);
    }

    [Test]
    public void OnStateExit_IsCalled_BeforeEntering_NewState()
    {
        string order = "";
        var sm = new StateMachine<PlayerState>(PlayerState.Idle);
        sm.OnStateExit(PlayerState.Idle, () => order += "exit");
        sm.OnStateEnter(PlayerState.Walking, () => order += "enter");
        sm.TransitionTo(PlayerState.Walking);
        Assert.That(order, Is.EqualTo("exitenter"));
    }

    [Test]
    public void OnStateEnter_IsNotCalled_ForOtherStates()
    {
        bool called = false;
        var sm = new StateMachine<PlayerState>(PlayerState.Idle);
        sm.OnStateEnter(PlayerState.Running, () => called = true);
        sm.TransitionTo(PlayerState.Walking);
        Assert.That(called, Is.False);
    }

    [Test]
    public void IsValidTransition_UnrestrictedMachine_AlwaysReturnsTrue()
    {
        var sm = new StateMachine<PlayerState>(PlayerState.Idle);
        Assert.That(sm.IsValidTransition(PlayerState.Idle, PlayerState.Combat), Is.True);
        Assert.That(sm.IsValidTransition(PlayerState.Jumping, PlayerState.Running), Is.True);
    }

    [Test]
    public void PlayerStateMachine_ValidTransition_Idle_To_Walking()
    {
        var sm = new PlayerStateMachine(PlayerState.Idle);
        sm.TransitionTo(PlayerState.Walking);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Walking));
    }

    [Test]
    public void PlayerStateMachine_ValidTransition_Walking_To_Running()
    {
        var sm = new PlayerStateMachine(PlayerState.Walking);
        sm.TransitionTo(PlayerState.Running);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Running));
    }

    [Test]
    public void PlayerStateMachine_ValidTransition_Running_To_Walking()
    {
        var sm = new PlayerStateMachine(PlayerState.Running);
        sm.TransitionTo(PlayerState.Walking);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Walking));
    }

    [Test]
    public void PlayerStateMachine_ValidTransition_Idle_To_Jumping()
    {
        var sm = new PlayerStateMachine(PlayerState.Idle);
        sm.TransitionTo(PlayerState.Jumping);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Jumping));
    }

    [Test]
    public void PlayerStateMachine_ValidTransition_Walking_To_Jumping()
    {
        var sm = new PlayerStateMachine(PlayerState.Walking);
        sm.TransitionTo(PlayerState.Jumping);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Jumping));
    }

    [Test]
    public void PlayerStateMachine_ValidTransition_Running_To_Jumping()
    {
        var sm = new PlayerStateMachine(PlayerState.Running);
        sm.TransitionTo(PlayerState.Jumping);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Jumping));
    }

    [Test]
    public void PlayerStateMachine_ValidTransition_Jumping_To_Idle()
    {
        var sm = new PlayerStateMachine(PlayerState.Jumping);
        sm.TransitionTo(PlayerState.Idle);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Idle));
    }

    [Test]
    public void PlayerStateMachine_ValidTransition_Idle_To_Combat()
    {
        var sm = new PlayerStateMachine(PlayerState.Idle);
        sm.TransitionTo(PlayerState.Combat);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Combat));
    }

    [Test]
    public void PlayerStateMachine_ValidTransition_Combat_To_Idle()
    {
        var sm = new PlayerStateMachine(PlayerState.Combat);
        sm.TransitionTo(PlayerState.Idle);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Idle));
    }

    [Test]
    public void TransitionTo_InvalidTransition_ThrowsInvalidOperationException()
    {
        var sm = new PlayerStateMachine(PlayerState.Jumping);
        Assert.That(() => sm.TransitionTo(PlayerState.Running),
            Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void TransitionTo_InvalidTransition_Running_To_Combat_Throws()
    {
        var sm = new PlayerStateMachine(PlayerState.Running);
        Assert.That(() => sm.TransitionTo(PlayerState.Combat),
            Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void TransitionTo_InvalidTransition_Combat_To_Walking_Throws()
    {
        var sm = new PlayerStateMachine(PlayerState.Combat);
        Assert.That(() => sm.TransitionTo(PlayerState.Walking),
            Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void IsValidTransition_ReturnsTrue_ForAllowedPairs()
    {
        var sm = new PlayerStateMachine();
        Assert.That(sm.IsValidTransition(PlayerState.Idle, PlayerState.Walking), Is.True);
        Assert.That(sm.IsValidTransition(PlayerState.Walking, PlayerState.Running), Is.True);
        Assert.That(sm.IsValidTransition(PlayerState.Jumping, PlayerState.Idle), Is.True);
    }

    [Test]
    public void IsValidTransition_ReturnsFalse_ForForbiddenPairs()
    {
        var sm = new PlayerStateMachine();
        Assert.That(sm.IsValidTransition(PlayerState.Jumping, PlayerState.Running), Is.False);
        Assert.That(sm.IsValidTransition(PlayerState.Running, PlayerState.Combat), Is.False);
        Assert.That(sm.IsValidTransition(PlayerState.Combat, PlayerState.Walking), Is.False);
    }

    [Test]
    public void CurrentState_DoesNotChange_OnFailedTransition()
    {
        var sm = new PlayerStateMachine(PlayerState.Jumping);
        try { sm.TransitionTo(PlayerState.Running); } catch (InvalidOperationException) { }
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Jumping));
    }

    // ── Self-transition behaviour ──────────────────────────────────────────────

    [Test]
    public void SelfTransition_UnrestrictedMachine_Succeeds()
    {
        // The unrestricted base class has null _allowedTransitions, so all
        // transitions — including self-transitions — are considered valid.
        var sm = new StateMachine<PlayerState>(PlayerState.Idle);
        Assert.That(() => sm.TransitionTo(PlayerState.Idle), Throws.Nothing);
        Assert.That(sm.CurrentState, Is.EqualTo(PlayerState.Idle));
    }

    [Test]
    public void SelfTransition_UnrestrictedMachine_TriggersExitAndEnterCallbacks()
    {
        string order = "";
        var sm = new StateMachine<PlayerState>(PlayerState.Idle);
        sm.OnStateExit(PlayerState.Idle, () => order += "exit");
        sm.OnStateEnter(PlayerState.Idle, () => order += "enter");

        sm.TransitionTo(PlayerState.Idle);

        Assert.That(order, Is.EqualTo("exitenter"));
    }

    [Test]
    public void SelfTransition_PlayerStateMachine_ThrowsInvalidOperationException()
    {
        // PlayerStateMachine defines an explicit allowed-transition set.
        // Self-transitions (e.g. Idle→Idle) are intentionally absent from that
        // set and must therefore be rejected.
        var sm = new PlayerStateMachine(PlayerState.Idle);
        Assert.That(() => sm.TransitionTo(PlayerState.Idle),
            Throws.TypeOf<InvalidOperationException>());
    }
}
