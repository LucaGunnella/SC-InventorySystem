using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StackUI : Draggable
{
    public SlotUI SlotUI { get; private set; }
    private Image _image;

    protected override void Awake() {
        base.Awake();
        _image = GetComponent<Image>();

        if (transform.parent.TryGetComponent<SlotUI>(out var slot))
            SetSlot(slot);
        else
            throw new NullReferenceException("Slot is null");
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);
        transform.position = Input.mousePosition;

        transform.SetParent(_canvas.transform);
        transform.SetAsLastSibling();

        _image.raycastTarget = false;

        SlotUI.SetStack(null);
    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);
        transform.SetParent(SlotUI.transform);
        _image.raycastTarget = true;
    }

    public void SetSlot(SlotUI slotUI) {
        SlotUI = slotUI;
        SlotUI.SetStack(this);
        transform.SetParent(SlotUI.transform);
    }

}