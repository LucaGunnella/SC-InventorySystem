using System;
using UnityEngine;

namespace SCI_LG
{

    public class Player : MonoBehaviour
    {
        [SerializeField] ItemData itemData;
        [SerializeField] InventoryUI inventoryUI;
        public Inventory Inventory { get; private set; }

        private void Awake() {
            Inventory = new Inventory(64);
            inventoryUI.Init(Inventory);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (itemData == null) throw new NullReferenceException();
                Inventory.AddItem(itemData, 1);
            }

            if (Input.GetKeyDown(KeyCode.I)) {
                inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
            }
        }

    }

}