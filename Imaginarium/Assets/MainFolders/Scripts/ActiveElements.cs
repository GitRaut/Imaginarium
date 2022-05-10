using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveElements : MonoBehaviour
{
    public Transform cross;
    public TMP_InputField input_field;
    public Button ready_button;
    public bool is_active;
    private bool check;


    private void Start()
    {
        is_active = false;
        check = false;
    }

    private void Update()
    {
        if (!is_active && !check)
        {
            cross.gameObject.SetActive(false);
            input_field.interactable = false;
            ready_button.interactable = false;
            check = true;
        }
        else if(is_active && check)
        {
            cross.gameObject.SetActive(true);
            check = false;
        }

        if (input_field.text != "" && is_active)
            ready_button.interactable = true;
    }
}
