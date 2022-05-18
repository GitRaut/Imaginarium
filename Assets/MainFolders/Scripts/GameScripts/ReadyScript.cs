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

    public Transform hand;
    public Transform cardField;
    public Transform crossButton;
    public Button ready_button;

    public void OnClick()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] && (TurnStates)PhotonNetwork.CurrentRoom.CustomProperties["turn_state"] == TurnStates.MP_CHOSING)
        {
            properties.Add("asoc", input_field.text);
            properties.Add("turn_state", TurnStates.P_CHOSING);

            //member card in selected cards
            CardScript card = cardField.GetChild(0).GetComponent<CardScript>();
            int[] cards = (int[])PhotonNetwork.CurrentRoom.CustomProperties["selected_cards"];
            cards[PhotonNetwork.LocalPlayer.ActorNumber - 1] = card.id;
            properties.Add("selected_cards", cards);

            //clear and update screen after choosing card
            cardField.GetChild(0).GetComponent<CardScript>().def_parent = hand;
            cardField.GetChild(0).SetParent(hand);
            crossButton.gameObject.SetActive(false);
            input_field.interactable = false;
            ready_button.interactable = false;
            input_field.text = "";

            for (int i = 0; i < hand.childCount; i++)
                hand.GetChild(i).GetComponent<CardScript>().is_used = false;

            gameManager.GiveCards(card.cardIndex, card.cardIndex + 1, PhotonNetwork.LocalPlayer);
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
