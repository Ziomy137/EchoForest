using System.Collections.Generic;

namespace EchoForest.Core;

/// <summary>Lookup contract for static item definitions.</summary>
public interface IItemDatabase
{
    ItemDefinition GetItem(string itemId);
    List<ItemDefinition> GetAllItems();
}