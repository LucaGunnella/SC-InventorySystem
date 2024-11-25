using UnityEngine;
using UnityEngine.EventSystems;

namespace SCI_LG
{

    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        protected Canvas _canvas;
        protected RectTransform _rectTransform;

        protected virtual void Awake() {
            _canvas = transform.GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public virtual void OnBeginDrag(PointerEventData eventData) { }

        public virtual void OnDrag(PointerEventData eventData) {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public virtual void OnEndDrag(PointerEventData eventData) { }

    }

}
