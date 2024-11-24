using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler
{
    private Canvas _canvas;
    private RectTransform _rectTransform;
    
    private void Awake() {
        _canvas = transform.parent.GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData) {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
}
