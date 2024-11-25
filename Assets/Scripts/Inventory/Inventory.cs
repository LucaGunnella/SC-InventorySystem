using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    private List<ItemStack> _stacks; // List of stacks in the inventory
    private int _maxStackSize;

    public Inventory(int maxStackSize) {
        _stacks = new List<ItemStack>();
        _maxStackSize = maxStackSize;
    }

    // Add items to the inventory, returns the number of leftover items if any
    public int AddItem(ItemData item, int quantity) {
        var remaining = quantity;

        // First, try adding to existing stacks with the same item
        foreach (var stack in _stacks) {
            if (stack.ItemData == item && stack.quantity < _maxStackSize) {
                var spaceAvailable = _maxStackSize - stack.quantity;
                var toAdd = Math.Min(spaceAvailable, remaining);
                stack.quantity += toAdd;
                remaining -= toAdd;

                if (remaining == 0) return 0; // All items were added
            }
        }

        // Then, create new stacks for the remaining items
        while (remaining > 0) {
            var toAdd = Math.Min(_maxStackSize, remaining);
            _stacks.Add(new ItemStack(item, toAdd));
            remaining -= toAdd;
        }

        PrintInventory();
        return remaining; // Return leftover items
    }

    // Remove items from the inventory, returns true if successful
    public bool TryRemoveItem(ItemData item, int quantity) {
        var toRemove = quantity;

        for (var i = 0; i < _stacks.Count; i++) {
            if (_stacks[i].ItemData == item) {
                if (_stacks[i].quantity > toRemove) {
                    _stacks[i].quantity -= toRemove;
                    return true; // Successfully removed all items
                }

                toRemove -= _stacks[i].quantity;
                _stacks.RemoveAt(i);
                i--; // Adjust index after removal
            }

            if (toRemove == 0) return true; // All items were removed
        }

        return false; // Not enough items to remove
    }

    // Get a read-only list of stacks for UI or other purposes
    public IReadOnlyList<ItemStack> GetStacks() {
        return _stacks.AsReadOnly();
    }

    private void PrintInventory() {
        var itemsString = string.Empty;
        foreach (var stack in _stacks) {
            itemsString += stack.ItemData.itemName + " " + stack.quantity + "\n";
        }

        var inventoryString = "Inventory:\n" + itemsString + "\n";
        Debug.Log(inventoryString);
    }

}

// Represents a stack of items in the inventory
[Serializable]
public class ItemStack
{

    public ItemData ItemData { get; private set; }
    public int quantity;

    public ItemStack(ItemData itemData, int quantity) {
        ItemData = itemData;
        this.quantity = quantity;
    }

}