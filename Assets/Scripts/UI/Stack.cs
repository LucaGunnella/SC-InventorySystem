using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stack : Draggable
{

    [NonSerialized] public int gridPosX;
    [NonSerialized] public int gridPosY;

    public Slot Slot { get; private set; }
    private Image _image;

    private void Awake() {
        _image = GetComponent<Image>();

        if (transform.parent.TryGetComponent<Slot>(out var slot))
            SetSlot(slot);
        else
            throw new NullReferenceException("Slot is null");
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);
        transform.position = Input.mousePosition;

        transform.SetParent(Slot.transform.parent);
        transform.SetAsLastSibling();

        _image.raycastTarget = false;

        Slot.SetStack(null);
    }

    public override void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);
        transform.SetParent(Slot.transform);
        _image.raycastTarget = true;
    }

    public void SetSlot(Slot slot) {
        Slot = slot;
        Slot.SetStack(this);
        transform.SetParent(Slot.transform);
    }

}