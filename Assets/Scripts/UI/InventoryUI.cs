using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SCI_LG
{

    public class InventoryUI : MonoBehaviour
    {

        [SerializeField] private GameObject _stackUIprefab;
        [SerializeField] private Button _orderHighToLowButton;
        [SerializeField] private Button _orderLowToHighButton;
        [SerializeField] private Button _aggregateHighToLowButton;
        [SerializeField] private Button _aggregateLowToHighButton;

        private List<SlotUI> slotUIs = new();
        private List<StackUI> stackUIs = new();

        private Inventory inventory;

        private List<StackUI> stackUIPool = new(); //pool of unused UI to be recycled

        public void Init(Inventory inventory) {
            this.inventory = inventory;

            inventory.OnAddItem += RefreshUI;
            inventory.OnRemoveItem += RemoveStackUIs;
            inventory.OnSort += RearrangeUI;
            
            _orderHighToLowButton.onClick.AddListener( () => inventory.OrderByMethod(OrderMethod.HIGH_TO_LOW));
            _orderLowToHighButton.onClick.AddListener( () => inventory.OrderByMethod(OrderMethod.LOW_TO_HIGH));
            _aggregateHighToLowButton.onClick.AddListener(() => inventory.AggregateAndOrderByMethod(OrderMethod.HIGH_TO_LOW));
            _aggregateLowToHighButton.onClick.AddListener(() => inventory.AggregateAndOrderByMethod(OrderMethod.LOW_TO_HIGH));
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
            RemoveStackUIs(stackUIs.Where(x => !itemStacks.Contains(x.ItemStack)));
        }

        /// <summary>
        /// Clears UI so that it can be rearranged by the Refresh.
        /// </summary>
        private void RearrangeUI() {
            ClearUI();
            RefreshUI();
        }

        /// <summary>
        /// Clears UI by removing each
        /// </summary>
        private void ClearUI() {
            RemoveStackUIs(stackUIs);
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
                stackUI.SetSlotUIOwner(slotUIs.First(x => x.transform.childCount == 0));
                stackUI.gameObject.SetActive(true);
            }
            else {
                var stackUI = Instantiate(_stackUIprefab, slotUIs.First(x => x.transform.childCount == 0).transform);
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

            existingStackUI.SetUI(itemStack); //sets stackUI with same itemStack but updated data
            
            return true;
        }

        /// <summary>
        /// Removes target stackUI (adds to pool the leftovers).
        /// </summary>
        /// <param name="stackUIsToRemove">stack to remove</param>
        private void RemoveStackUIs(IEnumerable<StackUI> stackUIsToRemove) {
            var toRemove = new List<StackUI>();

            foreach (var stackUI in stackUIsToRemove) {
                stackUI.SetUI(null);
                stackUI.transform.SetParent(transform);
                stackUI.gameObject.SetActive(false);
                stackUIPool.Add(stackUI);
                toRemove.Add(stackUI);
            }

            foreach (var stackUI in toRemove) {
                stackUIs.Remove(stackUI);
            }
        }

        /// <summary>
        /// Removes stackUi with assigned itemStack that is no longer in inventory (adds to pool the leftovers).
        /// </summary>
        /// <param name="itemStacks">itemStack to check</param>
        private void RemoveStackUIs(IEnumerable<ItemStack> itemStacks) {
            var toRemove = new List<StackUI>();

            foreach (var stack in itemStacks) {
                var stackUI = stackUIPool.FirstOrDefault(x => x.ItemStack == stack);
                if (stackUI == null) {
                    throw new NullReferenceException("Stack UI could not be found");
                }

                stackUI.SetUI(null);
                stackUI.transform.SetParent(transform);
                stackUI.gameObject.SetActive(false);
                stackUIPool.Add(stackUI);
                toRemove.Add(stackUI);
            }

            foreach (var stackUI in toRemove) {
                stackUIs.Remove(stackUI);
            }
        }

        #endregion

    }

}