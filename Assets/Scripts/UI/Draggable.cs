using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas _canvas;
    private RectTransform _rectTransform;
    
    private void Awake() {
        _canvas = transform.parent.GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
    }
    
    public virtual void OnBeginDrag(PointerEventData eventData) {
    }
    
    public virtual void OnDrag(PointerEventData eventData) {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public virtual void OnEndDrag(PointerEventData eventData) {
    }

}
