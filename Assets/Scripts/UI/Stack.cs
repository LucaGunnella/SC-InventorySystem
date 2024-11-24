using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stack : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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

    public void OnBeginDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;

        transform.SetParent(Slot.transform.parent);
        transform.SetAsLastSibling();

        _image.raycastTarget = false;

        Slot.SetStack(null);
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        transform.SetParent(Slot.transform);
        _image.raycastTarget = true;
    }

    public void SetSlot(Slot slot) {
        Slot = slot;
        Slot.SetStack(this);
        transform.SetParent(Slot.transform);
    }

}