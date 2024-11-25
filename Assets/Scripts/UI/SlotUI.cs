using UnityEngine;
using UnityEngine.EventSystems;

namespace SCI_LG
{

    public class SlotUI : MonoBehaviour, IDropHandler
    {

        private StackUI _stackUIowned;

        public void OnDrop(PointerEventData eventData) {
            if (!eventData.pointerDrag.TryGetComponent<StackUI>(out var droppedStack)) return;

            //switches stack with other item slot
            if (_stackUIowned != null) {
                _stackUIowned.SetSlot(droppedStack.SlotUIOwner);
            }

            droppedStack.SetSlot(this);
        }

        public void SetStack(StackUI stackUI) {
            this._stackUIowned = stackUI;
        }

    }

}