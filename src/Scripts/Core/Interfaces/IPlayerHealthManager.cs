namespace EchoForest.Core;

/// <summary>Player health state and damage/healing operations.</summary>
public interface IPlayerHealthManager
{
    float MaxHealth { get; }
    float CurrentHealth { get; }
    bool IsDead { get; }
    void TakeDamage(float amount);
    void Heal(float amount);
}