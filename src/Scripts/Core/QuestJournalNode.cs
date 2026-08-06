using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;

namespace EchoForest.Core;

/// <summary>Godot overlay that renders the quest journal and owns its input.</summary>
[ExcludeFromCodeCoverage(Justification = "Godot CanvasLayer wrapper - requires scene tree")]
public partial class QuestJournalNode : CanvasLayer
{
    private QuestJournalController _controller = null!;
    private IQuestDatabase _questDatabase = null!;
    private VBoxContainer _activeQuestList = null!;
    private VBoxContainer _completedQuestList = null!;
    private Label _titleLabel = null!;
    private Label _descriptionLabel = null!;
    private VBoxContainer _objectivesList = null!;
    private string? _selectedQuestId;

    public override void _Ready()
    {
        var cottage = GetTree().CurrentScene as CottageAreaNode
            ?? throw new InvalidOperationException("QuestJournalNode requires CottageAreaNode as the current scene.");

        _questDatabase = cottage.QuestDatabase;
        _activeQuestList = GetNode<VBoxContainer>("Center/Panel/Layout/QuestListPanel/QuestList/ActiveQuestList");
        _completedQuestList = GetNode<VBoxContainer>("Center/Panel/Layout/QuestListPanel/QuestList/CompletedQuestList");
        _titleLabel = GetNode<Label>("Center/Panel/Layout/DetailsPanel/Details/TitleLabel");
        _descriptionLabel = GetNode<Label>("Center/Panel/Layout/DetailsPanel/Details/DescriptionLabel");
        _objectivesList = GetNode<VBoxContainer>("Center/Panel/Layout/DetailsPanel/Details/ObjectivesList");

        GetNode<Button>("Center/Panel/Layout/DetailsPanel/Details/CloseButton").Pressed += Close;
        _controller = new QuestJournalController(cottage.EventBus);
        _controller.Changed += RefreshJournal;
        _controller.Synchronize(cottage.QuestService.GetQuestStates());
        Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed(InputActionNames.Inventory)
            && !(Visible && @event.IsActionPressed(InputActionNames.Pause)))
        {
            return;
        }

        if (Visible)
            Close();
        else
            Open();

        GetViewport().SetInputAsHandled();
    }

    public override void _ExitTree()
    {
        if (Visible)
            SetPlayerInputEnabled(true);

        _controller.Changed -= RefreshJournal;
        _controller.Dispose();
    }

    private void Open()
    {
        Visible = true;
        SetPlayerInputEnabled(false);
        RefreshJournal();
    }

    private void Close()
    {
        Visible = false;
        SetPlayerInputEnabled(true);
    }

    private void RefreshJournal()
    {
        EnsureSelection();
        PopulateQuestList(_activeQuestList, _controller.ActiveQuestIds, new Color(1f, 0.843f, 0f, 1f));
        PopulateQuestList(_completedQuestList, _controller.CompletedQuestIds, new Color(0.55f, 0.55f, 0.55f, 1f));
        RefreshDetails();
    }

    private void EnsureSelection()
    {
        if (_selectedQuestId is not null
            && (_controller.ActiveQuestIds.Contains(_selectedQuestId)
                || _controller.CompletedQuestIds.Contains(_selectedQuestId)))
        {
            return;
        }

        _selectedQuestId = _controller.ActiveQuestIds.Count > 0
            ? _controller.ActiveQuestIds[0]
            : _controller.CompletedQuestIds.Count > 0 ? _controller.CompletedQuestIds[0] : null;
    }

    private void PopulateQuestList(VBoxContainer container, IReadOnlyList<string> questIds, Color color)
    {
        ClearChildren(container);
        foreach (var questId in questIds)
        {
            var quest = _questDatabase.GetQuest(questId);
            var button = new Button
            {
                Text = quest.Title,
                Alignment = HorizontalAlignment.Left,
            };
            button.AddThemeColorOverride("font_color", color);
            button.Pressed += () =>
            {
                _selectedQuestId = questId;
                RefreshJournal();
            };
            container.AddChild(button);
        }
    }

    private void RefreshDetails()
    {
        ClearChildren(_objectivesList);
        if (_selectedQuestId is null)
        {
            _titleLabel.Text = "No quests yet";
            _descriptionLabel.Text = "Quest updates will appear here.";
            return;
        }

        var quest = _questDatabase.GetQuest(_selectedQuestId);
        _titleLabel.Text = quest.Title;
        _descriptionLabel.Text = quest.Description;
        foreach (var objective in quest.Objectives)
        {
            var isCompleted = _controller.IsObjectiveCompleted(quest.Id, objective.Id)
                || _controller.CompletedQuestIds.Contains(quest.Id);
            var checkbox = new CheckBox
            {
                Text = objective.Text,
                ButtonPressed = isCompleted,
                Disabled = true,
            };
            checkbox.AddThemeColorOverride("font_color", isCompleted
                ? new Color(0.55f, 0.55f, 0.55f, 1f)
                : new Color(0.878f, 0.863f, 0.824f, 1f));
            _objectivesList.AddChild(checkbox);
        }
    }

    private void SetPlayerInputEnabled(bool enabled)
    {
        var player = GetTree().CurrentScene?.GetNodeOrNull<Node>("Player");
        if (player is not null)
            player.ProcessMode = enabled ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
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