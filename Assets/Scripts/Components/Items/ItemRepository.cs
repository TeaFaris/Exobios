using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public sealed class ItemRepository
{
    private readonly ReadOnlyCollection<Item> items;

    public ItemRepository(IList<Item> items)
    {
        this.items = new ReadOnlyCollection<Item>(items);
    }

    public Item[] SetupInventory(PlayerInventory to)
    {
        var newItems = new Item[items.Count];

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];

            var newItem = Object.Instantiate(item, to.transform);

            newItems[i] = newItem;
        }

        return newItems;
    }
}