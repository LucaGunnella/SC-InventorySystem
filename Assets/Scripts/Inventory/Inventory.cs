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
        public event Action<IEnumerable<ItemStack>> OnRemoveItem;

        public event Action OnSort;

        public Inventory(int maxStackSize) {
            _stacks = new List<ItemStack>();
            _maxStackSize = maxStackSize;
        }

        #region Inventory management

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
                    var newItemStack = CreateItemStack(itemData);
                    _stacks.Add(newItemStack);
                    OnAddItem?.Invoke(newItemStack);
                    remaining -= 1;
                }

                return remaining;
            }

            // Tries to add remaining quantity to existing stacks
            foreach (var stack in _stacks) {
                if (stack.ItemData != itemData || stack.Quantity >= _maxStackSize) continue;

                var spaceAvailable = _maxStackSize - stack.Quantity;
                var toAdd = Math.Min(spaceAvailable, remaining);
                stack.Quantity += toAdd;
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
                var newItemStack = CreateItemStack(itemData);
                _stacks.Add(newItemStack);
                OnAddItem?.Invoke(newItemStack);
                remaining -= toAdd;
            }

            PrintInventory();
            return remaining; // Return leftover items
        }

        private ItemStack CreateItemStack(ItemData itemData) {
            var newItemStack = new ItemStack(itemData, 1);
            newItemStack.OnConsumed += DestroyItemStack;
            return newItemStack;
        }

        private void DestroyItemStack(ItemStack itemStack) {
            itemStack.OnConsumed -= DestroyItemStack;
            _stacks.Remove(itemStack);
            OnRemoveItem?.Invoke(new List<ItemStack> { itemStack });
        }

        /// <summary>
        /// Remove items from the inventory, returns true if successful
        /// </summary>
        /// <param name="item">type of item to remove</param>
        /// <param name="quantity">number of items to remove</param>
        /// <returns></returns>
        public bool TryRemoveQuantity(ItemData item, int quantity) {
            var toRemove = quantity;

            for (var i = 0; i < _stacks.Count; i++) {
                if (_stacks[i].ItemData == item) {
                    if (_stacks[i].Quantity > toRemove) {
                        _stacks[i].Quantity -= toRemove;
                        return true; // Successfully removed all items
                    }

                    toRemove -= _stacks[i].Quantity;
                    _stacks.RemoveAt(i);
                    i--; // Adjust index after removal
                }

                if (toRemove == 0) {
                    return true;
                } // All items were removed
            }

            return false; // Not enough items to remove
        }


        /// <summary>
        /// Aggregates existing stacks to compact inventory.
        /// </summary>
        private void AggregateStacks() {
            // Dictionary to group quantities by ItemData
            var aggregatedData = new Dictionary<ItemData, int>();

            // Aggregate quantities for each ItemData
            foreach (var stack in _stacks) {
                if (aggregatedData.ContainsKey(stack.ItemData)) {
                    aggregatedData[stack.ItemData] += stack.Quantity;
                }
                else {
                    aggregatedData[stack.ItemData] = stack.Quantity;
                }
            }

            // Rebuild the itemStacks list with aggregated data
            _stacks = new List<ItemStack>();
            foreach (var kvp in aggregatedData) {
                AddItem(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Orders stacks based on given method.
        /// </summary>
        /// <param name="method"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void OrderByMethod(OrderMethod method) {
            switch (method) {
                case OrderMethod.LOW_TO_HIGH:
                    _stacks.Sort((a, b) => a.Quantity - b.Quantity);
                    break;
                case OrderMethod.HIGH_TO_LOW:
                    _stacks.Sort((a, b) => -a.Quantity - b.Quantity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }

            OnSort?.Invoke();
        }

        /// <summary>
        /// Aggregates stacks and then orders them based on given method. 
        /// </summary>
        /// <param name="method"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AggregateAndOrderByMethod(OrderMethod method) {
            switch (method) {
                case OrderMethod.LOW_TO_HIGH:
                    AggregateStacks();
                    _stacks.Sort((a, b) => a.Quantity - b.Quantity);
                    break;
                case OrderMethod.HIGH_TO_LOW:
                    AggregateStacks();
                    _stacks.Sort((a, b) => -a.Quantity - b.Quantity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }

            OnSort?.Invoke();
        }

        #endregion

        // Get a read-only list of stacks for UI
        public IReadOnlyList<ItemStack> GetStacks() {
            return _stacks.AsReadOnly();
        }

        private void PrintInventory() {
            var itemsString = string.Empty;
            foreach (var stack in _stacks) {
                itemsString += stack.ItemData.itemName + " " + stack.Quantity + "\n";
            }

            var inventoryString = "Inventory:\n" + itemsString + "\n";
            Debug.Log(inventoryString);
        }

    }

    [Serializable]
    public class ItemStack
    {

        public ItemData ItemData { get; private set; }
        private int _quantity;

        public int Quantity {
            get => _quantity;
            set {
                _quantity = value;
                if(_quantity <= 0)
                    OnConsumed?.Invoke(this);
            }
        }

        public event Action<ItemStack> OnConsumed;

        public ItemStack(ItemData itemData, int quantity) {
            ItemData = itemData;
            Quantity = quantity;
        }

    }

    public enum OrderMethod
    {

        LOW_TO_HIGH,
        HIGH_TO_LOW

    }

}