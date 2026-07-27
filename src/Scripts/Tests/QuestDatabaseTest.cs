using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class QuestDatabaseTest
{
    [Test]
    public void GetAllQuests_LoadsEveryQuestJsonFile()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, string>
        {
            ["res://src/Assets/Data/Quests/q_kidnapped.json"] = """
            { "id": "q_kidnapped", "title": "Kidnapped", "objectives": [{ "id": "wake_up", "text": "Wake up", "required": true }], "rewards": [] }
            """,
            ["res://src/Assets/Data/Quests/q_seek_mage.json"] = """
            { "id": "q_seek_mage", "title": "Seek the Mage", "objectives": [{ "id": "talk_to_mage", "text": "Talk to the mage", "required": true }], "rewards": [] }
            """,
        });
        var database = new QuestDatabase(fileSystem);

        var quests = database.GetAllQuests();

        Assert.That(quests.Select(quest => quest.Id), Is.EqualTo(["q_kidnapped", "q_seek_mage"]));
    }

    [Test]
    public void GetQuest_ReturnsQuestById()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, string>
        {
            ["res://src/Assets/Data/Quests/q_kidnapped.json"] = """
            { "id": "q_kidnapped", "title": "Kidnapped", "objectives": [{ "id": "wake_up", "text": "Wake up", "required": true }], "rewards": [] }
            """,
        });
        var database = new QuestDatabase(fileSystem);

        var quest = database.GetQuest("q_kidnapped");

        Assert.Multiple(() =>
        {
            Assert.That(quest.Title, Is.EqualTo("Kidnapped"));
            Assert.That(quest.Objectives[0].Id, Is.EqualTo("wake_up"));
        });
    }
}