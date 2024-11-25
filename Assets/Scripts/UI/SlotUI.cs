using UnityEngine;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IDropHandler
{

    private StackUI stackUI;

    public void OnDrop(PointerEventData eventData) {
        if(!eventData.pointerDrag.TryGetComponent<StackUI>(out var droppedStack)) return;
        
        //switches stack with other item slot
        if (stackUI != null) {
            stackUI.SetSlot(droppedStack.SlotUI);
        }
        
        droppedStack.SetSlot(this);
    }

    public void SetStack(StackUI stackUI) {
        this.stackUI = stackUI;
    }

}