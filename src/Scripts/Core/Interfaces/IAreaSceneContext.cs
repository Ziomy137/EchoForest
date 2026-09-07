namespace EchoForest.Core;

/// <summary>Shared gameplay services exposed by every loaded area scene.</summary>
public interface IAreaSceneContext
{
    IEventBus EventBus { get; }
    IInputHandler InputHandler { get; }
    IAreaTransitionService AreaTransitionService { get; }
    IQuestService QuestService { get; }
}