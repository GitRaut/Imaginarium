using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PFieldScript : MonoBehaviour, IDropHandler
{
    public Transform hand;
    public Transform cross_button;
    public Button ready_button;

    public void OnDrop(PointerEventData eventData)
    {
        CardScript card = eventData.pointerDrag.GetComponent<CardScript>();

        if (card)
        {
            for (int i = 0; i < hand.childCount; i++)
                hand.GetChild(i).GetComponent<CardScript>().is_used = true;

            ready_button.interactable = true;
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
        ready_button.interactable = false;

        for (int i = 0; i < hand.childCount; i++)
            hand.GetChild(i).GetComponent<CardScript>().is_used = false;
    }
}
