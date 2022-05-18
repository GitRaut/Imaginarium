using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class PButtonsScript : MonoBehaviour
{
    public GameManagerScript gameManager;
    public Transform cardField;
    public Transform hand;
    public Transform crossButton;
    public Button readyButton;

    public void OnClickPChoosing()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

        if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] && (TurnStates)PhotonNetwork.CurrentRoom.CustomProperties["turn_state"] == TurnStates.P_CHOSING)
        {
            properties.Add("turn_state", TurnStates.VOTING);
            playerProperties.Add("isReady", true);

            //member card in selected cards
            CardScript card = cardField.GetChild(0).GetComponent<CardScript>();
            int[] selected_cards = (int[])PhotonNetwork.CurrentRoom.CustomProperties["selected_cards"];
            selected_cards[PhotonNetwork.LocalPlayer.ActorNumber] = card.id;
            properties.Add("selected_cards", selected_cards);

            //clear and update screen after choosing card
            cardField.GetChild(0).GetComponent<CardScript>().def_parent = hand;
            cardField.GetChild(0).SetParent(hand);
            crossButton.gameObject.SetActive(false);
            readyButton.interactable = false;
            for (int i = 0; i < hand.childCount; i++)
                hand.GetChild(i).GetComponent<CardScript>().is_used = false;
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    public void OnClickVote()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

        if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] && (TurnStates)PhotonNetwork.CurrentRoom.CustomProperties["turn_state"] == TurnStates.VOTING)
        {
            properties.Add("turn_state", TurnStates.RESULTS);
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    public void OnClickResult()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

        if ((TurnStates)PhotonNetwork.CurrentRoom.CustomProperties["turn_state"] == TurnStates.RESULTS)
        {
            properties.Add("turn_state", TurnStates.MP_CHOSING);
            if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"])
            {
                gameManager.SetTurn(PhotonNetwork.LocalPlayer.GetNext());
            }
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
}
