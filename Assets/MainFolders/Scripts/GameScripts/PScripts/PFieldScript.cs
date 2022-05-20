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
    public bool is_select;

    public void OnDrop(PointerEventData eventData)
    {
        CardScript card = eventData.pointerDrag.GetComponent<CardScript>();

<<<<<<< Updated upstream
        if (card)
=======
        if (card && !card.is_used && !is_select)
>>>>>>> Stashed changes
        {
            for (int i = 0; i < hand.childCount; i++)
                hand.GetChild(i).GetComponent<CardScript>().is_used = true;

            ready_button.interactable = true;
            cross_button.gameObject.SetActive(true);

            card.def_parent = transform;
            card.is_used = true;
        }
    }

    public void OnClickCross()
    {
        transform.GetChild(0).GetComponent<CardScript>().def_parent = hand;
        transform.GetChild(0).SetParent(hand);

        cross_button.gameObject.SetActive(false);
        ready_button.interactable = false;

        for (int i = 0; i < hand.childCount; i++)
            hand.GetChild(i).GetComponent<CardScript>().is_used = false;
    }
}
