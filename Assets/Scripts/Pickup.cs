using System;
using System.Collections;
using UnityEngine;

namespace SCI_LG
{

    public class Pickup : MonoBehaviour
    {

        private SpriteRenderer _model;
        private PickupData _pickupData;
        private bool _pickedUp = false;
        
        public event Action OnPickedUp;

        private void Awake() {
            _model = GetComponentInChildren<SpriteRenderer>();
        }

        public void Init(PickupData pickupData) {
            _pickupData = pickupData;
            _model.sprite = pickupData.itemData.icon;
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player") || _pickedUp) return;

            Debug.Log("other.name");
            StartCoroutine(Collect(other.transform.GetComponent<Player>()));

        }

        private IEnumerator Collect(Player player) {
            _pickedUp = true;
            _model.gameObject.SetActive(false);
            player.Inventory.AddItem(_pickupData.itemData, _pickupData.amount);
            yield return null;
            
            OnPickedUp?.Invoke();
            yield return null;
            
            Destroy(gameObject);
        }

    }

}

