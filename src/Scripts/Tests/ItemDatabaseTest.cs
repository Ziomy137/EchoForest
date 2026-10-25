using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using EchoForest.Core;

namespace EchoForest.Tests;

[TestFixture]
public class ItemDatabaseTest
{
    private const string SampleCatalog = """
        [
          {
            "id": "item_tincture_small",
            "display_name": "Small Tincture",
            "description": "Restores 25 health.",
            "item_type": "Consumable",
            "max_stack_size": 10,
            "icon_texture_path": ""
          },
          {
            "id": "item_sword_basic",
            "display_name": "Basic Sword",
            "description": "A simple iron sword.",
            "item_type": "Weapon",
            "max_stack_size": 1,
            "icon_texture_path": ""
          }
        ]
        """;

    [Test]
    public void GetItem_ReturnsDefinitionById()
    {
        var database = CreateDatabase(SampleCatalog);

        var item = database.GetItem("item_sword_basic");

        Assert.Multiple(() =>
        {
            Assert.That(item.DisplayName, Is.EqualTo("Basic Sword"));
            Assert.That(item.Description, Is.EqualTo("A simple iron sword."));
            Assert.That(item.ItemType, Is.EqualTo(ItemType.Weapon));
            Assert.That(item.MaxStackSize, Is.EqualTo(1));
            Assert.That(item.IconTexturePath, Is.Empty);
        });
    }

    [Test]
    public void GetAllItems_ReturnsSortedDefinitions()
    {
        var items = CreateDatabase(SampleCatalog).GetAllItems();

        Assert.That(items.Select(item => item.Id), Is.EqualTo(new[]
        {
            "item_sword_basic",
            "item_tincture_small",
        }));
    }

    [Test]
    public void GetAllItems_ReturnsDefensiveList()
    {
        var database = CreateDatabase(SampleCatalog);
        var items = database.GetAllItems();
        items.Clear();

        Assert.That(database.GetAllItems(), Has.Count.EqualTo(2));
    }

    [Test]
    public void GetItem_UnknownId_ThrowsKeyNotFoundException()
    {
        var database = CreateDatabase(SampleCatalog);

        Assert.Throws<KeyNotFoundException>(() => database.GetItem("missing_item"));
    }

    [Test]
    public void GetItem_WhitespaceId_ThrowsArgumentException()
    {
        var database = CreateDatabase(SampleCatalog);

        Assert.Throws<System.ArgumentException>(() => database.GetItem(" "));
    }

    [Test]
    public void GetAllItems_LoadsTheEightStartingItemsFromAsset()
    {
        var repositoryRoot = FindRepositoryRoot();
        var filePath = Path.Combine(repositoryRoot, "src", "Assets", "Data", "Items", "items.json");
        var fileSystem = new MockFileSystem(new Dictionary<string, string>
        {
            [ItemDatabase.DefaultFilePath] = File.ReadAllText(filePath),
        });

        var items = new ItemDatabase(fileSystem).GetAllItems();

        Assert.That(items.Select(item => item.Id), Is.EquivalentTo(new[]
        {
            "item_sword_basic",
            "item_bow_basic",
            "item_shield_basic",
            "item_tunic_peasant",
            "item_tunic_leather",
            "item_tincture_small",
            "item_tincture_large",
            "item_portal_key",
        }));
    }

    [Test]
    public void StartingCatalog_ClassifiesShieldWithWeapons()
    {
        var repositoryRoot = FindRepositoryRoot();
        var filePath = Path.Combine(repositoryRoot, "src", "Assets", "Data", "Items", "items.json");
        var fileSystem = new MockFileSystem(new Dictionary<string, string>
        {
            [ItemDatabase.DefaultFilePath] = File.ReadAllText(filePath),
        });

        var shield = new ItemDatabase(fileSystem).GetItem("item_shield_basic");

        Assert.Multiple(() =>
        {
            Assert.That(shield.ItemType, Is.EqualTo(ItemType.Weapon));
            Assert.That(shield.EquipmentSlot, Is.EqualTo(EquipmentSlot.OffHand));
        });
    }

    [Test]
    public void StartingCatalog_ProvidesTinctureHealingAmounts()
    {
        var repositoryRoot = FindRepositoryRoot();
        var filePath = Path.Combine(repositoryRoot, "src", "Assets", "Data", "Items", "items.json");
        var fileSystem = new MockFileSystem(new Dictionary<string, string>
        {
            [ItemDatabase.DefaultFilePath] = File.ReadAllText(filePath),
        });
        var database = new ItemDatabase(fileSystem);

        Assert.Multiple(() =>
        {
            Assert.That(database.GetItem("item_tincture_small").HealthRestore, Is.EqualTo(25f));
            Assert.That(database.GetItem("item_tincture_large").HealthRestore, Is.EqualTo(75f));
        });
    }

    [Test]
    public void AllItems_HaveExistingIconTextures()
    {
        var repositoryRoot = FindRepositoryRoot();
        var filePath = Path.Combine(repositoryRoot, "src", "Assets", "Data", "Items", "items.json");
        var fileSystem = new MockFileSystem(new Dictionary<string, string>
        {
            [ItemDatabase.DefaultFilePath] = File.ReadAllText(filePath),
        });
        var items = new ItemDatabase(fileSystem).GetAllItems();

        foreach (var item in items)
        {
            var iconFilePath = Path.Combine(repositoryRoot,
                item.IconTexturePath["res://".Length..].Replace('/', Path.DirectorySeparatorChar));
            Assert.That(File.Exists(iconFilePath), Is.True, $"Missing icon for {item.Id}: {item.IconTexturePath}");
        }
    }

    [Test]
    public void Constructor_UsesDefaultCatalogPath()
    {
        var database = new ItemDatabase(new MockFileSystem(new Dictionary<string, string>
        {
            [ItemDatabase.DefaultFilePath] = SampleCatalog,
        }));

        Assert.That(database.GetItem("item_sword_basic").Id, Is.EqualTo("item_sword_basic"));
    }

    [Test]
    public void InvalidJson_ThrowsItemLoadException()
    {
        var database = CreateDatabase("not valid json");

        Assert.Throws<ItemLoadException>(() => database.GetAllItems());
    }

    [Test]
    public void MissingCatalog_ThrowsItemLoadException()
    {
        var database = new ItemDatabase(new MockFileSystem());

        Assert.Throws<ItemLoadException>(() => database.GetAllItems());
    }

    [Test]
    public void JsonNull_ThrowsItemLoadException()
    {
        Assert.Throws<ItemLoadException>(() => CreateDatabase("null").GetAllItems());
    }

    [Test]
    public void EmptyCatalog_ThrowsItemLoadException()
    {
        Assert.Throws<ItemLoadException>(() => CreateDatabase("[]").GetAllItems());
    }

    [Test]
    public void NullDefinition_ThrowsItemLoadException()
    {
        Assert.Throws<ItemLoadException>(() => CreateDatabase("[null]").GetAllItems());
    }

    [TestCase("id")]
    [TestCase("display_name")]
    [TestCase("description")]
    public void MissingRequiredField_ThrowsItemLoadException(string missingField)
    {
        var definition = missingField switch
        {
            "id" => """{"display_name":"Test Item","description":"Test description","item_type":"Weapon","max_stack_size":1,"icon_texture_path":""}""",
            "display_name" => """{"id":"item_test","description":"Test description","item_type":"Weapon","max_stack_size":1,"icon_texture_path":""}""",
            _ => """{"id":"item_test","display_name":"Test Item","item_type":"Weapon","max_stack_size":1,"icon_texture_path":""}""",
        };

        Assert.Throws<ItemLoadException>(() => CreateDatabase($"[{definition}]").GetAllItems());
    }

    [Test]
    public void UnknownItemType_ThrowsItemLoadException()
    {
        const string catalog = """
            [{ "id": "item_test", "display_name": "Test Item", "description": "Test description", "item_type": "Unknown", "max_stack_size": 1, "icon_texture_path": "" }]
            """;

        Assert.Throws<ItemLoadException>(() => CreateDatabase(catalog).GetAllItems());
    }

    [Test]
    public void DuplicateIds_ThrowItemLoadException()
    {
        const string duplicateCatalog = """
            [
              { "id": "item_sword_basic", "display_name": "Sword", "description": "A sword.", "item_type": "Weapon", "max_stack_size": 1, "icon_texture_path": "" },
              { "id": "item_sword_basic", "display_name": "Another Sword", "description": "Another sword.", "item_type": "Weapon", "max_stack_size": 1, "icon_texture_path": "" }
            ]
            """;

        Assert.Throws<ItemLoadException>(() => CreateDatabase(duplicateCatalog).GetAllItems());
    }

    [TestCase("0")]
    [TestCase("-1")]
    public void InvalidMaxStackSize_ThrowsItemLoadException(string maxStackSize)
    {
        var catalog = SampleCatalog.Replace("\"max_stack_size\": 1,", $"\"max_stack_size\": {maxStackSize},");

        Assert.Throws<ItemLoadException>(() => CreateDatabase(catalog).GetAllItems());
    }

    [Test]
    public void InventoryItem_StoresDefinitionAndPositiveQuantity()
    {
        var definition = CreateDatabase(SampleCatalog).GetItem("item_sword_basic");

        var item = new InventoryItem(definition, 3);

        Assert.Multiple(() =>
        {
            Assert.That(item.Definition, Is.SameAs(definition));
            Assert.That(item.Quantity, Is.EqualTo(3));
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void InventoryItem_NonPositiveQuantity_Throws(int quantity)
    {
        var definition = CreateDatabase(SampleCatalog).GetItem("item_sword_basic");

        Assert.Throws<System.ArgumentOutOfRangeException>(() => new InventoryItem(definition, quantity));
    }

    [Test]
    public void InventoryItem_NullDefinition_ThrowsArgumentNullException() =>
        Assert.Throws<System.ArgumentNullException>(() => new InventoryItem(null!, 1));

    [Test]
    public void Constructor_NullFileSystem_ThrowsArgumentNullException() =>
        Assert.Throws<System.ArgumentNullException>(() => new ItemDatabase(null!));

    [Test]
    public void Constructor_WhitespacePath_ThrowsArgumentException() =>
        Assert.Throws<System.ArgumentException>(() => new ItemDatabase(new MockFileSystem(), " "));

    private static ItemDatabase CreateDatabase(string catalog) => new(new MockFileSystem(new Dictionary<string, string>
    {
        [ItemDatabase.DefaultFilePath] = catalog,
    }));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "EchoForest.sln")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the repository root from the NUnit test directory.");
    }
}