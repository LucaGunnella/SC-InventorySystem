using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCI_LG
{

    public class StackUI : Draggable
    {

        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _quantityText;
        
        private ItemStack _itemStack;
        
        public SlotUI SlotUIOwner { get; private set; }

        protected override void Awake() {
            base.Awake();
            _image = GetComponent<Image>();

            if (transform.parent.TryGetComponent<SlotUI>(out var slot))
                SetSlot(slot);
            else
                throw new NullReferenceException("Slot is null");
        }

        public void SetUI(ItemStack itemStack) {
            _image.sprite = itemStack.ItemData.icon;
            _quantityText.text = itemStack.quantity.ToString();
            _itemStack = itemStack;
        }

        public override void OnBeginDrag(PointerEventData eventData) {
            base.OnBeginDrag(eventData);
            transform.position = Input.mousePosition;

            transform.SetParent(_canvas.transform);
            transform.SetAsLastSibling();

            _image.raycastTarget = false;

            SlotUIOwner.SetStack(null);
        }

        public override void OnEndDrag(PointerEventData eventData) {
            base.OnEndDrag(eventData);
            transform.SetParent(SlotUIOwner.transform);
            _image.raycastTarget = true;
        }

        public void SetSlot(SlotUI slotUI) {
            SlotUIOwner = slotUI;
            SlotUIOwner.SetStack(this);
            transform.SetParent(SlotUIOwner.transform);
        }

    }

}