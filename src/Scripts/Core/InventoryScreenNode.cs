using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace EchoForest.Core;

/// <summary>Godot overlay for browsing the 20-slot player inventory.</summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper - requires scene tree")]
public partial class InventoryScreenNode : CanvasLayer
{
    private IInventoryManager _inventory = null!;
    private IEventBus _eventBus = null!;
    private GridContainer _slotGrid = null!;
    private TextureRect _selectedIcon = null!;
    private Label _selectedName = null!;
    private Label _selectedType = null!;
    private Label _selectedQuantity = null!;
    private Label _selectedDescription = null!;
    private Button _useButton = null!;
    private Button _equipButton = null!;
    private InventoryItem? _selectedItem;

    public event Action? Closed;

    public void CloseScreen() => Close();

    public void Configure(IInventoryManager inventory, IEventBus eventBus)
    {
        _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public override void _Ready()
    {
        if (_inventory is null || _eventBus is null)
            throw new InvalidOperationException("InventoryScreenNode must be configured before entering the scene tree.");

        ProcessMode = ProcessModeEnum.Always;
        _slotGrid = GetNode<GridContainer>("Center/Panel/Layout/Content/SlotsPanel/Slots");
        _selectedIcon = GetNode<TextureRect>("Center/Panel/Layout/Content/DetailsPanel/Details/SelectedIcon");
        _selectedName = GetNode<Label>("Center/Panel/Layout/Content/DetailsPanel/Details/SelectedName");
        _selectedType = GetNode<Label>("Center/Panel/Layout/Content/DetailsPanel/Details/SelectedType");
        _selectedQuantity = GetNode<Label>("Center/Panel/Layout/Content/DetailsPanel/Details/SelectedQuantity");
        _selectedDescription = GetNode<Label>("Center/Panel/Layout/Content/DetailsPanel/Details/SelectedDescription");
        _useButton = GetNode<Button>("Center/Panel/Layout/Content/DetailsPanel/Details/Actions/UseButton");
        _equipButton = GetNode<Button>("Center/Panel/Layout/Content/DetailsPanel/Details/Actions/EquipButton");

        GetNode<Button>("Center/Panel/Layout/Header/CloseButton").Pressed += Close;
        _useButton.Pressed += RequestUse;
        _equipButton.Pressed += RequestEquip;
        _inventory.Changed += RefreshInventory;
        RefreshInventory();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(InputActionNames.Pause))
        {
            GetViewport().SetInputAsHandled();
            Close();
        }
    }

    public override void _ExitTree()
    {
        if (_inventory is not null)
            _inventory.Changed -= RefreshInventory;
    }

    private void RefreshInventory()
    {
        ClearChildren(_slotGrid);
        var items = _inventory.GetAllItems();
        if (_selectedItem is null || !items.Contains(_selectedItem))
            _selectedItem = items.Count > 0 ? items[0] : null;

        for (var index = 0; index < _inventory.MaxSlots; index++)
        {
            if (index < items.Count)
                _slotGrid.AddChild(CreateItemSlot(items[index]));
            else
                _slotGrid.AddChild(CreateEmptySlot());
        }

        RefreshDetails();
        GetNode<Label>("Center/Panel/Layout/Footer/SlotCountLabel").Text =
            $"{_inventory.GetOccupiedSlots()} / {_inventory.MaxSlots} slots";
    }

    private Button CreateItemSlot(InventoryItem item)
    {
        var button = new Button
        {
            CustomMinimumSize = new Vector2(110f, 84f),
            TooltipText = $"{item.Definition.DisplayName}\n{item.Definition.Description}\n{item.Definition.ItemType}",
        };
        var icon = new TextureRect
        {
            Texture = GD.Load<Texture2D>(item.Definition.IconTexturePath),
            Position = new Vector2(37f, 5f),
            Size = new Vector2(36f, 36f),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        var name = new Label
        {
            Text = item.Definition.DisplayName,
            Position = new Vector2(3f, 44f),
            Size = new Vector2(104f, 18f),
            HorizontalAlignment = HorizontalAlignment.Center,
            ClipText = true,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        name.AddThemeFontSizeOverride("font_size", 10);
        var quantity = new Label
        {
            Text = item.Quantity > 1 ? item.Quantity.ToString() : string.Empty,
            Position = new Vector2(83f, 62f),
            Size = new Vector2(22f, 16f),
            HorizontalAlignment = HorizontalAlignment.Right,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        quantity.AddThemeColorOverride("font_color", Palette.Gold);
        button.AddChild(icon);
        button.AddChild(name);
        button.AddChild(quantity);
        button.Pressed += () =>
        {
            _selectedItem = item;
            RefreshDetails();
        };
        return button;
    }

    private static Button CreateEmptySlot() => new()
    {
        CustomMinimumSize = new Vector2(110f, 84f),
        Disabled = true,
        FocusMode = Control.FocusModeEnum.None,
    };

    private void RefreshDetails()
    {
        var item = _selectedItem;
        if (item is null)
        {
            _selectedIcon.Texture = null;
            _selectedName.Text = "Select an item";
            _selectedType.Text = string.Empty;
            _selectedQuantity.Text = string.Empty;
            _selectedDescription.Text = "Choose an occupied slot to see item details.";
            _useButton.Visible = false;
            _equipButton.Visible = false;
            return;
        }

        _selectedIcon.Texture = GD.Load<Texture2D>(item.Definition.IconTexturePath);
        _selectedName.Text = item.Definition.DisplayName;
        _selectedType.Text = item.Definition.ItemType.ToString();
        _selectedQuantity.Text = $"Quantity: {item.Quantity}";
        _selectedDescription.Text = item.Definition.Description;
        _useButton.Visible = item.Definition.ItemType == ItemType.Consumable;
        _equipButton.Visible = item.Definition.ItemType is ItemType.Weapon or ItemType.Armor;
    }

    private void RequestUse()
    {
        if (_selectedItem is not null)
            _eventBus.Publish(new ItemUseRequestedEvent(_selectedItem.Definition.Id));
    }

    private void RequestEquip()
    {
        if (_selectedItem is not null)
            _eventBus.Publish(new ItemEquipRequestedEvent(_selectedItem.Definition.Id));
    }

    private void Close()
    {
        Closed?.Invoke();
        QueueFree();
    }

    private static void ClearChildren(Node parent)
    {
        foreach (var child in parent.GetChildren())
        {
            parent.RemoveChild(child);
            child.QueueFree();
        }
    }
}