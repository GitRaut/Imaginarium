using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadyScript : MonoBehaviour
{
    public Transform next_screen;
    public TMP_InputField input_field;

    private Button ready_button;

    public void OnClick()
    {
        Debug.Log("DONE");
        GameManagerScript.Instance.asoc = input_field.text;
        // next_screen.gameObject.SetActive(true);
        // transform.gameObject.SetActive(false);
    }

    private void Start()
    {
        ready_button = transform.GetComponent<Button>();
    }

    private void Update()
    {
        if(input_field.text != "")
            ready_button.interactable = true;
        else
            ready_button.interactable = false;
    }
}
