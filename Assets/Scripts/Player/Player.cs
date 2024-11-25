using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] ItemData itemData;
    public Inventory Inventory{get; private set;}

    private void Awake() {
        Inventory = new Inventory(64);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Inventory.AddItem(itemData, 1);
        }
    }

}
