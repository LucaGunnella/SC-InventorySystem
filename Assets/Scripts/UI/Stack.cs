using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stack : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [NonSerialized] public int gridPosX;
    [NonSerialized] public int gridPosY;

    public void OnBeginDrag(PointerEventData eventData) {
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
    }

}
