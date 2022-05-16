using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ReadyScript : MonoBehaviour
{
    public Transform next_screen;
    public TMP_InputField input_field;
    public GameManagerScript gameManager;

    private Button ready_button;

    public void OnClick()
    {
        Debug.Log("DONE");
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add("asoc", input_field.text);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        // gameManager.asoc = input_field.text;
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
