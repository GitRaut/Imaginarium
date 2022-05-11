using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PFieldScript : MonoBehaviour, IDropHandler
{
    public Transform hand;
    public Transform script_handler;

    public void OnDrop(PointerEventData eventData)
    {
        CardScript card = eventData.pointerDrag.GetComponent<CardScript>();

        if (card)
        {
            for (int i = 0; i < hand.childCount; i++)
                hand.GetChild(i).GetComponent<CardScript>().is_used = true;

            script_handler.GetComponent<PActiveElements>().is_active = true;

            card.def_parent = transform;
            card.is_used = true;
            card.transform.position = transform.position;
        }
    }
}
