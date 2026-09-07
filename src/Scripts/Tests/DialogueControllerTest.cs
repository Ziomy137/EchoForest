using System.Collections.Generic;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class DialogueControllerTest
{
    private const string WifeDialogueJson = """
        {
          "npc_id": "wife",
          "is_story_critical": true,
          "lines": [
            { "id": "wife_01", "speaker": "Wife", "text": "First line", "next": "wife_02" },
            { "id": "wife_02", "speaker": "Wife", "text": "Second line", "next": null }
          ]
        }
        """;

    private const string OptionalDialogueJson = """
                {
                    "npc_id": "wife",
                    "is_story_critical": false,
                    "lines": [
                        { "id": "wife_01", "speaker": "Wife", "text": "Optional line", "next": null }
                    ]
                }
                """;

    private static DialogueService CreateService(string json = WifeDialogueJson)
    {
        var files = new Dictionary<string, string>
        {
            ["res://src/Assets/Data/dialogues/wife.json"] = json,
        };
        return new DialogueService(new MockFileSystem(files));
    }

    [Test]
    public void StartConversation_RaisesStartedAndFirstLineChanged()
    {
        var controller = new DialogueController(CreateService());
        var started = false;
        DialogueLine? changedLine = null;
        controller.OnConversationStarted += () => started = true;
        controller.OnLineChanged += line => changedLine = line;

        controller.StartConversation("wife");

        Assert.Multiple(() =>
        {
            Assert.That(started, Is.True);
            Assert.That(controller.IsConversationActive, Is.True);
            Assert.That(controller.CurrentLine!.Id, Is.EqualTo("wife_01"));
            Assert.That(changedLine, Is.SameAs(controller.CurrentLine));
        });
    }

    [Test]
    public void StartConversationAtLine_SetsRequestedDialogueLine()
    {
        var controller = new DialogueController(CreateService());

        controller.StartConversationAtLine("wife", "wife_02");

        Assert.Multiple(() =>
        {
            Assert.That(controller.IsConversationActive, Is.True);
            Assert.That(controller.CurrentLine!.Id, Is.EqualTo("wife_02"));
        });
    }

    [Test]
    public void Advance_MovesToNextLine()
    {
        var controller = new DialogueController(CreateService());
        controller.StartConversation("wife");

        var advanced = controller.Advance();

        Assert.Multiple(() =>
        {
            Assert.That(advanced, Is.True);
            Assert.That(controller.CurrentLine!.Id, Is.EqualTo("wife_02"));
            Assert.That(controller.IsConversationActive, Is.True);
        });
    }

    [Test]
    public void Advance_AtLastLine_EndsConversationAndAutoSavesCriticalDialogue()
    {
        var saves = new MockSaveDataService();
        var controller = new DialogueController(CreateService(), saves, () => new SaveData { CurrentArea = "cottage" });
        var ended = false;
        controller.OnConversationEnded += () => ended = true;
        controller.StartConversation("wife");
        controller.Advance();

        var advanced = controller.Advance();

        Assert.Multiple(() =>
        {
            Assert.That(advanced, Is.False);
            Assert.That(ended, Is.True);
            Assert.That(controller.IsConversationActive, Is.False);
            Assert.That(controller.CurrentLine, Is.Null);
            Assert.That(saves.SaveWasCalled, Is.True);
            Assert.That(saves.LastSavedData!.CurrentArea, Is.EqualTo("cottage"));
            Assert.That(saves.LastSavedSlot, Is.EqualTo(1));
        });
    }

    [Test]
    public void EndConversation_WhenInactive_DoesNotRaiseEndedEvent()
    {
        var controller = new DialogueController(CreateService());
        var ended = false;
        controller.OnConversationEnded += () => ended = true;

        controller.EndConversation();

        Assert.That(ended, Is.False);
    }

    [Test]
    public void Advance_AtLastLine_DoesNotAutoSaveOptionalDialogue()
    {
        var saves = new MockSaveDataService();
        var controller = new DialogueController(CreateService(OptionalDialogueJson), saves, () => new SaveData());
        controller.StartConversation("wife");

        controller.Advance();

        Assert.That(saves.SaveWasCalled, Is.False);
    }
}