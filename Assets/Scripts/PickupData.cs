using UnityEngine;

namespace SCI_LG
{

    [CreateAssetMenu(fileName = "newPickupData", menuName = "Asset/PickupData")]
    public class PickupData : ScriptableObject
    {
        public ItemData itemData;
        [Range(1, 64)]
        public int amount;
    }

}

