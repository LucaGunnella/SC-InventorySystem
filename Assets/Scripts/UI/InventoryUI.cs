using System.Collections.Generic;
using UnityEngine;

namespace SCI_LG
{

    public class InventoryUI : MonoBehaviour
    {

        [SerializeField] private GameObject stackUIprefab;
        private List<SlotUI> slots = new();
        private Inventory inventory;

        private void Awake() {
            slots = new List<SlotUI>(transform.GetComponentsInChildren<SlotUI>());
            gameObject.SetActive(false);
        }

        private void OnEnable() {
            var itemStacks = inventory.GetStacks();
            for (var i = 0; i < itemStacks.Count; i++) {
                var stackUIscript = Instantiate(stackUIprefab, slots[i].transform).GetComponent<StackUI>();
                stackUIscript.SetUI(itemStacks[i]);
            }
        }

        public void Init(Inventory inventory) {
            this.inventory = inventory;
        }

    }

}
