using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PActiveElements : MonoBehaviour
{
    public Button ready_button;
    public Transform cross;
    public bool is_active;
    private bool check;

    private void Start()
    {
        is_active = false;
        check = false;
    }

    void Update()
    {
        if (!is_active && !check)
        {
            cross.gameObject.SetActive(false);
            ready_button.interactable = false;
            check = true;
        }
        else if (is_active && check)
        {
            cross.gameObject.SetActive(true);
            ready_button.interactable = true;
            check = false;
        }
    }
}
