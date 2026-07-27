using System;
using System.Collections.Generic;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class DialogueServiceTest
{
    private const string WifeDialogueJson = """
        {
          "npc_id": "wife",
          "is_story_critical": true,
          "lines": [
            {
              "id": "wife_01",
              "speaker": "Wife",
              "text": "Please, find our child!",
              "next": "wife_02"
            },
            {
              "id": "wife_02",
              "speaker": "Wife",
              "text": "Head to the city and find the local mage.",
              "next": null
            }
          ]
        }
        """;

    private DialogueService CreateService(string json = WifeDialogueJson)
    {
        var files = new Dictionary<string, string>
        {
            ["res://src/Assets/Data/dialogues/wife.json"] = json,
        };
        return new DialogueService(new MockFileSystem(files));
    }

    [Test]
    public void LoadDialogue_DeserializesNpcAndFirstLine()
    {
        var service = CreateService();

        var dialogue = service.LoadDialogue("wife");

        Assert.Multiple(() =>
        {
            Assert.That(dialogue.NpcId, Is.EqualTo("wife"));
            Assert.That(dialogue.IsStoryCritical, Is.True);
            Assert.That(dialogue.FirstLineId, Is.EqualTo("wife_01"));
            Assert.That(dialogue.Lines, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void GetLine_ReturnsCorrectText()
    {
        var service = CreateService();
        service.LoadDialogue("wife");

        var line = service.GetLine("wife_01");

        Assert.Multiple(() =>
        {
            Assert.That(line.Speaker, Is.EqualTo("Wife"));
            Assert.That(line.Text, Is.EqualTo("Please, find our child!"));
            Assert.That(line.NextLineId, Is.EqualTo("wife_02"));
        });
    }

    [Test]
    public void GetNextLine_FollowsChain()
    {
        var service = CreateService();
        service.LoadDialogue("wife");

        var next = service.GetNextLine("wife_01");

        Assert.That(next!.Id, Is.EqualTo("wife_02"));
    }

    [Test]
    public void GetNextLine_ReturnsNullAtEnd()
    {
        var service = CreateService();
        service.LoadDialogue("wife");

        Assert.That(service.GetNextLine("wife_02"), Is.Null);
    }

    [Test]
    public void LoadDialogue_InvalidJson_ThrowsDialogueLoadException()
    {
        var service = CreateService("{invalid json");

        Assert.Throws<DialogueLoadException>(() => service.LoadDialogue("wife"));
    }

    [Test]
    public void LoadDialogue_MissingFile_ThrowsDialogueLoadException()
    {
        var service = new DialogueService(new MockFileSystem());

        Assert.Throws<DialogueLoadException>(() => service.LoadDialogue("wife"));
    }

    [Test]
    public void GetLine_UnknownId_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        service.LoadDialogue("wife");

        Assert.Throws<KeyNotFoundException>(() => service.GetLine("missing"));
    }
}