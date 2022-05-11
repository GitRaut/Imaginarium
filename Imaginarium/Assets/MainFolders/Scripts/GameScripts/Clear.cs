using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Clear : MonoBehaviour
{
    public Transform field;
    public Transform hand;
    public Transform script_handler;

    public void OnClick()
    {
        field.GetChild(1).SetParent(hand);

        for (int i = 0; i < hand.childCount; i++)
            hand.GetChild(i).GetComponent<CardScript>().is_used = false;

        script_handler.GetComponent<ActiveElements>().is_active = false;
    }
}
