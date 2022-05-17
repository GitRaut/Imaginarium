using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ReadyScript : MonoBehaviour
{
    public TMP_InputField input_field;
    public GameManagerScript gameManager;

    private Button ready_button;

    public void OnClick()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] && (TurnStates)PhotonNetwork.CurrentRoom.CustomProperties["turn_state"] == TurnStates.MP_CHOSING)
        {
            properties.Add("asoc", input_field.text);
            properties.Add("turn_state", TurnStates.P_CHOSING);
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
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
