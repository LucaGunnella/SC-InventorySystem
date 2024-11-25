using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCI_LG
{

    [CreateAssetMenu(fileName = "newItemData", menuName = "Asset/ItemData")]
    public class ItemData : ScriptableObject
    {

        public string itemName;
        public Sprite icon;
        public bool stackable = true;
        public List<Effect> effects;

    }

}