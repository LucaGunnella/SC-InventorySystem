using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCI_LG
{

    public class PickupSpawner : MonoBehaviour
    {

        [SerializeField] private GameObject _pickupPrefab;
        [SerializeField] private List<PickupData> _pickupDatas = new();
        private Player _player;
        private Bounds _bounds;

        private void Awake() {
            _player = FindObjectOfType<Player>();

            if (_player == null) {
                throw new NullReferenceException("Player not found");
            }

            _bounds = new Bounds {
                center = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z),
                size = new Vector3(15, 0, 15)
            };
        }

        private void Start() {
            SpawnPickup();
        }

        private void SpawnPickup() {
            var randomPositionInBounds = GetRandomPositionInBounds();
            
            while (Vector3.Distance(_player.transform.position, randomPositionInBounds) < 3f) {
                randomPositionInBounds = GetRandomPositionInBounds();
            }

            var pickup = Instantiate(_pickupPrefab, randomPositionInBounds, Quaternion.identity);
            var pickupScript = pickup.GetComponent<Pickup>();
            
            pickupScript.Init(_pickupDatas[Random.Range(0, _pickupDatas.Count)]);
            pickupScript.OnPickedUp += SpawnPickup;
        }

        private Vector3 GetRandomPositionInBounds() {
            return new Vector3(
                Random.Range(_bounds.min.x, _bounds.max.x), 
                Random.Range(_bounds.min.y, _bounds.max.y),
                Random.Range(_bounds.min.z, _bounds.max.z));
        }

    }

}