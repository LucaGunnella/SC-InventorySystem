using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCI_LG
{

    public class StackUI : Draggable, IPointerClickHandler
    {

        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _quantityText;

        public ItemStack ItemStack { get; private set; }

        public SlotUI SlotUIOwner { get; private set; }
        
        public event Action<StackUI> OnSelect;

        protected override void Awake() {
            base.Awake();
            _image = GetComponent<Image>();

            if (transform.parent.TryGetComponent<SlotUI>(out var slot))
                SetSlotUIOwner(slot);
            else
                throw new NullReferenceException("Slot is null");
        }

        #region Setter methods
        public void SetUI(ItemStack itemStack) {
            if (itemStack == null) {
                _image.sprite = default;
                _quantityText.text = default;
                ItemStack = null;
                SlotUIOwner.SetStackUIOwned(null);
            }
            else {
                _image.sprite = itemStack.ItemData.icon;
                _quantityText.text = itemStack.Quantity.ToString();
                ItemStack = itemStack;
            }
        }

        public void SetSlotUIOwner(SlotUI slotUI) {
            SlotUIOwner = slotUI;
            SlotUIOwner.SetStackUIOwned(this);
            transform.SetParent(SlotUIOwner.transform);
        }
        
        #endregion

        #region Draggable implementation

        public override void OnBeginDrag(PointerEventData eventData) {
            base.OnBeginDrag(eventData);

            transform.SetParent(_canvas.transform);
            transform.SetAsLastSibling();

            _image.raycastTarget = false;

            SlotUIOwner.SetStackUIOwned(null);
        }

        public override void OnEndDrag(PointerEventData eventData) {
            base.OnEndDrag(eventData);
            transform.SetParent(SlotUIOwner.transform);
            _image.raycastTarget = true;
        }

        #endregion

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Right) {
                OnSelect?.Invoke(this);
            }
        }

    }

}