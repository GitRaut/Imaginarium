using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class FieldScript : MonoBehaviour, IDropHandler
{
    public Transform hand;
    public Transform cross_button;
    public Button ready_button;
    public TMP_InputField input_field;

    public void OnDrop(PointerEventData eventData)
    {
        CardScript card = eventData.pointerDrag.GetComponent<CardScript>();

        if (card)
        {
            for(int i = 0; i < hand.childCount; i++)
                hand.GetChild(i).GetComponent<CardScript>().is_used = true;

            input_field.interactable = true;
            cross_button.gameObject.SetActive(true);

            card.def_parent = transform;
            card.is_used = true;
            card.transform.position = transform.position;
        }
    }

    public void OnClickCross()
    {
        transform.GetChild(1).SetParent(hand);

        cross_button.gameObject.SetActive(false);
        input_field.interactable = false;
        ready_button.interactable = false;
        input_field.text = "";

        for (int i = 0; i < hand.childCount; i++)
            hand.GetChild(i).GetComponent<CardScript>().is_used = false;
    }
}
