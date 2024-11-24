using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{

    private Stack _stack;

    public void OnDrop(PointerEventData eventData) {
        if(!eventData.pointerDrag.TryGetComponent<Stack>(out var droppedStack)) return;
        
        //switches stack with other item slot
        if (_stack != null) {
            _stack.SetSlot(droppedStack.Slot);
        }
        
        droppedStack.SetSlot(this);
    }

    public void SetStack(Stack stack) {
        _stack = stack;
    }

}