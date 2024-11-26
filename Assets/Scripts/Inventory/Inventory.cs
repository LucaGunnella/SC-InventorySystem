using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCI_LG
{

    public class Inventory
    {

        private List<ItemStack> _stacks; // List of stacks in the inventory
        private int _maxStackSize;
        
        public event Action<ItemStack> OnAddItem;
        public event Action<ItemStack> OnRemoveItem;

        public Inventory(int maxStackSize) {
            _stacks = new List<ItemStack>();
            _maxStackSize = maxStackSize;
        }

        /// <summary>
        /// Add items to the inventory, returns the number of leftover items if any
        /// </summary>
        /// <param name="itemData">type of item to add</param>
        /// <param name="quantity">number of items to add</param>
        /// <returns></returns>
        public int AddItem(ItemData itemData, int quantity) {
            var remaining = quantity;

            // If not stackable it creates a separate stack for each
            if (!itemData.stackable) {
                while (remaining > 0) {
                    var newItemStack = new ItemStack(itemData, 1);
                    _stacks.Add(newItemStack);
                    OnAddItem?.Invoke(newItemStack);
                    remaining -= 1;
                }
                return remaining;
            }
            
            // Tries to add remaining quantity to existing stacks
            foreach (var stack in _stacks) {
                if (stack.ItemData != itemData || stack.quantity >= _maxStackSize) continue;
                
                var spaceAvailable = _maxStackSize - stack.quantity;
                var toAdd = Math.Min(spaceAvailable, remaining);
                stack.quantity += toAdd;
                remaining -= toAdd;

                if (remaining == 0) {
                    OnAddItem?.Invoke(stack);
                    PrintInventory();
                    return 0;
                } // All items were added
            }

            // Then, create new stacks for the remaining items
            while (remaining > 0) {
                var toAdd = Math.Min(_maxStackSize, remaining);
                var newItemStack = new ItemStack(itemData, toAdd);
                _stacks.Add(newItemStack);
                OnAddItem?.Invoke(newItemStack);
                remaining -= toAdd;
            }

            PrintInventory();
            return remaining; // Return leftover items
        }

        /// <summary>
        /// Remove items from the inventory, returns true if successful
        /// </summary>
        /// <param name="item">type of item to remove</param>
        /// <param name="quantity">number of items to remove</param>
        /// <returns></returns>
        public bool TryRemoveItem(ItemData item, int quantity) {
            var toRemove = quantity;

            for (var i = 0; i < _stacks.Count; i++) {
                if (_stacks[i].ItemData == item) {
                    if (_stacks[i].quantity > toRemove) {
                        _stacks[i].quantity -= toRemove;
                        OnRemoveItem?.Invoke(_stacks[i]);
                        return true; // Successfully removed all items
                    }

                    toRemove -= _stacks[i].quantity;
                    _stacks.RemoveAt(i);
                    i--; // Adjust index after removal
                }

                if (toRemove == 0) {
                    OnRemoveItem?.Invoke(_stacks[i]);
                    return true;
                } // All items were removed
            }

            return false; // Not enough items to remove
        }

        // Get a read-only list of stacks for UI
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

}