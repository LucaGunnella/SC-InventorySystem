using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCI_LG
{

    public class InventoryUI : MonoBehaviour
    {

        [SerializeField] private GameObject stackUIprefab;

        private List<SlotUI> slotUIs = new();
        private List<StackUI> stackUIs = new();

        private Inventory inventory;

        private List<StackUI> stackUIPool; //pool of unused UI to be recycled

        public void Init(Inventory inventory) {
            this.inventory = inventory;

            inventory.OnAddItem += RefreshUI;
            inventory.OnRemoveItem += RemoveStackUI;
            inventory.OnSort += RearrangeUI;
        }

        #region UnityEvent functions

        //Fills the slot lists then disables GO
        private void Awake() {
            slotUIs = new List<SlotUI>(transform.GetComponentsInChildren<SlotUI>());
            gameObject.SetActive(false);
        }

        private void OnEnable() {
            if (inventory == null) return;

            RefreshUI();
        }

        #endregion

        #region Refresh methods

        /// <summary>
        /// Specific Refresh that accounts only for provided itemStack.
        /// </summary>
        /// <param name="itemStack">target itemStack to check</param>
        private void RefreshUI(ItemStack itemStack) {
            if (TryUpdateExistingUI(itemStack)) return;

            CreateStackUI(itemStack);
        }

        /// <summary>
        /// Generic Refresh that accounts for all itemStacks from inventory.
        /// </summary>
        private void RefreshUI() {
            var itemStacks = inventory.GetStacks(); //gets all stacks from inventory

            foreach (var t in itemStacks) {
                if (TryUpdateExistingUI(t)) continue; //skips to next element due to not needing an update

                CreateStackUI(t); //finds first empty slotUI and instantiates a stackUI (uses pooled object if any)
            }

            //checks if any stackUI has to be removed and adds any leftover to pool
            foreach (var stackUI in stackUIs.Where(x => !itemStacks.Contains(x.ItemStack))) {
                RemoveStackUI(stackUI);
            }
        }

        /// <summary>
        /// Clears UI so that it can be rearranged by the Refresh.
        /// </summary>
        private void RearrangeUI() {
            ClearUI();
            RefreshUI();
        }

        private void ClearUI() {
            foreach (var stackUI in stackUIs) {
                RemoveStackUI(stackUI);
            }
        }

        #endregion

        #region StackUI methods

        /// <summary>
        /// Creates a stackUI. First tries to draw from a pool, if unsuccessful instantiates new GO.
        /// </summary>
        /// <param name="itemStack">itemStack to assign to newly created UI</param>
        private void CreateStackUI(ItemStack itemStack) {
            if (stackUIPool.Any()) {
                var stackUI = stackUIPool.First();
                stackUIPool.Remove(stackUI);
                var stackUIScript = stackUI.GetComponent<StackUI>();

                stackUIScript.SetUI(itemStack);
                stackUIs.Add(stackUIScript);
                stackUI.transform.SetParent(slotUIs.First(x => x.transform.childCount == 0).transform);
                stackUI.gameObject.SetActive(true);
            }
            else {
                var stackUI = Instantiate(stackUIprefab, slotUIs.First(x => x.transform.childCount == 0).transform);
                var stackUIScript = stackUI.GetComponent<StackUI>();

                stackUIScript.SetUI(itemStack);
                stackUIs.Add(stackUIScript);
            }
        }

        /// <summary>
        /// Checks if any existing stackUI already represents the target itemStack and updates it.
        /// </summary>
        /// <param name="itemStack">target itemStack to add</param>
        /// <returns></returns>
        private bool TryUpdateExistingUI(ItemStack itemStack) {
            var existingStackUI =
                stackUIs.FirstOrDefault(x => x.ItemStack == itemStack); //checks if stack is already in UI
            if (existingStackUI == null) return false;

            if (existingStackUI.ItemStack.quantity != itemStack.quantity) {
                //checks if quantity is changed
                existingStackUI.SetUI(itemStack); //sets stackUI with same itemStack but updated data
            }

            return true;
        }

        /// <summary>
        /// Removes target stackUI (adds to pool the leftovers).
        /// </summary>
        /// <param name="stackUI">stack to remove</param>
        private void RemoveStackUI(StackUI stackUI) {
            stackUI.SetUI(null);
            stackUI.transform.SetParent(transform);
            stackUI.gameObject.SetActive(false);
            stackUIPool.Add(stackUI);
        }

        /// <summary>
        /// Removes stackUi with assigned itemStack that is no longer in inventory (adds to pool the leftovers).
        /// </summary>
        /// <param name="itemStack">itemStack to check</param>
        private void RemoveStackUI(ItemStack itemStack) {
            var stackUI = stackUIPool.FirstOrDefault(x => x.ItemStack == itemStack);
            if (stackUI == null) {
                throw new NullReferenceException("Stack UI could not be found");
            }

            stackUI.SetUI(null);
            stackUI.transform.SetParent(transform);
            stackUI.gameObject.SetActive(false);
            stackUIPool.Add(stackUI);
        }

        #endregion

    }

}