using System;

namespace EchoForest.Core;

/// <summary>Pure-C# player health model that publishes health and death events.</summary>
public sealed class PlayerHealthManager : IPlayerHealthManager
{
    private readonly IEventBus _eventBus;

    public PlayerHealthManager(float maxHealth, IEventBus eventBus)
    {
        if (maxHealth <= 0f)
            throw new ArgumentOutOfRangeException(nameof(maxHealth), "Maximum health must be greater than zero.");

        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public float MaxHealth { get; }
    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    public void TakeDamage(float amount)
    {
        ValidateAmount(amount);
        if (IsDead)
            return;

        CurrentHealth = Math.Max(0f, CurrentHealth - amount);
        _eventBus.Publish(new PlayerHealthChangedEvent(CurrentHealth, MaxHealth));
        if (CurrentHealth == 0f)
            _eventBus.Publish(new PlayerDiedEvent());
    }

    public void Heal(float amount)
    {
        ValidateAmount(amount);
        if (IsDead)
            return;

        var healedHealth = Math.Min(MaxHealth, CurrentHealth + amount);
        if (healedHealth == CurrentHealth)
            return;

        CurrentHealth = healedHealth;
        _eventBus.Publish(new PlayerHealthChangedEvent(CurrentHealth, MaxHealth));
    }

    private static void ValidateAmount(float amount)
    {
        if (amount <= 0f || float.IsNaN(amount))
            throw new ArgumentOutOfRangeException(nameof(amount), "Health amount must be greater than zero.");
    }
}