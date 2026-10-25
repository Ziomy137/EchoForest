using System;

namespace EchoForest.Core;

/// <summary>Consumes usable inventory items and applies their configured health recovery.</summary>
public sealed class InventoryItemUseService : IDisposable
{
    private readonly IInventoryManager _inventory;
    private readonly IPlayerHealthManager _playerHealth;
    private readonly IEventBus _eventBus;
    private bool _disposed;

    public InventoryItemUseService(IInventoryManager inventory, IPlayerHealthManager playerHealth, IEventBus eventBus)
    {
        _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        _playerHealth = playerHealth ?? throw new ArgumentNullException(nameof(playerHealth));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _eventBus.Subscribe<ItemUseRequestedEvent>(OnItemUseRequested);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _eventBus.Unsubscribe<ItemUseRequestedEvent>(OnItemUseRequested);
        _disposed = true;
    }

    private void OnItemUseRequested(ItemUseRequestedEvent gameEvent)
    {
        var item = _inventory.GetItem(gameEvent.ItemId);
        if (item is null
            || item.Definition.ItemType != ItemType.Consumable
            || item.Definition.HealthRestore is not float restoreAmount
            || restoreAmount <= 0f
            || _playerHealth.CurrentHealth >= _playerHealth.MaxHealth
            || _playerHealth.IsDead)
        {
            return;
        }

        if (_inventory.RemoveItem(gameEvent.ItemId))
            _playerHealth.Heal(restoreAmount);
    }
}