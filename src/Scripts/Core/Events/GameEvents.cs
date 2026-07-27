namespace EchoForest.Core;

/// <summary>Published when the player's health changes.</summary>
public sealed record PlayerHealthChangedEvent(float NewHealth, float MaxHealth);

/// <summary>Published when a quest becomes active.</summary>
public sealed record QuestStartedEvent(string QuestId);

/// <summary>Published when one quest objective is completed.</summary>
public sealed record QuestObjectiveCompletedEvent(string QuestId, string ObjectiveId);

/// <summary>Published when every required objective for a quest is complete.</summary>
public sealed record QuestCompletedEvent(string QuestId);

/// <summary>Published when the player begins interacting with an NPC.</summary>
public sealed record NpcInteractionStartedEvent(string NpcId);

/// <summary>Published after an area transition is requested.</summary>
public sealed record AreaTransitionEvent(string FromArea, string ToArea);

/// <summary>Published when the player reaches zero health.</summary>
public sealed record PlayerDiedEvent;

/// <summary>Published after items are added to the inventory.</summary>
public sealed record ItemPickedUpEvent(string ItemId, int Quantity);