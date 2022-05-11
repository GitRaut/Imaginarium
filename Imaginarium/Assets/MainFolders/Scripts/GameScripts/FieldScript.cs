using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FieldScript : MonoBehaviour, IDropHandler
{
    public Transform script_handler;
    public Transform hand;
    public TMP_InputField input_field;

    public void OnDrop(PointerEventData eventData)
    {
        CardScript card = eventData.pointerDrag.GetComponent<CardScript>();

        if (card)
        {
            for(int i = 0; i < hand.childCount; i++)
                hand.GetChild(i).GetComponent<CardScript>().is_used = true;

            script_handler.GetComponent<ActiveElements>().is_active = true;
            input_field.interactable = true;

            card.def_parent = transform;
            card.is_used = true;
            card.transform.position = transform.position;
        }
    }
}
