using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SC_DropArea : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        SC_CardDisplay _card = eventData.pointerDrag.GetComponent<SC_CardDisplay>();

        if (_card != null)
            _card.returnTo = this.transform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;
        
        SC_CardDisplay _card = eventData.pointerDrag.GetComponent<SC_CardDisplay>();

        if (_card != null)
            _card.parantHolder = this.transform;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        SC_CardDisplay _card = eventData.pointerDrag.GetComponent<SC_CardDisplay>();

        if (_card != null && _card.parantHolder == this.transform)
            _card.parantHolder = _card.returnTo;
    }
}
