using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{

    private Stack _stack;

    public void OnDrop(PointerEventData eventData) {
        var droppedStack = eventData.pointerDrag.GetComponent<Stack>();
        
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