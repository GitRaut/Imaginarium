using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class PButtonsScript : MonoBehaviour
{
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
            // properties.Add("turn_state", TurnStates.VOTING);
            //playerProperties.Add("isReady", true);

            //member card in selected cards
            CardScript card = cardField.GetChild(0).GetComponent<CardScript>();
            int[] selected_cards = (int[])PhotonNetwork.CurrentRoom.CustomProperties["selected_cards"];
            selected_cards[PhotonNetwork.LocalPlayer.ActorNumber - 1] = card.id;
            properties.Add("selected_cards", selected_cards);
            playerProperties.Add("selectedCard", card.id);

            //clear and update screen after choosing card
            cardField.GetChild(0).GetComponent<CardScript>().def_parent = hand;
            cardField.GetChild(0).gameObject.SetActive(false);
            cardField.GetChild(0).SetParent(hand);
            crossButton.gameObject.SetActive(false);
            readyButton.interactable = false;
            for (int i = 0; i < hand.childCount; i++)
                hand.GetChild(i).GetComponent<CardScript>().is_used = false;

            int[] myCards = (int[])PhotonNetwork.LocalPlayer.CustomProperties["myCards"];
            for (int i = 0; i < myCards.Length; i++)
            {
                if (myCards[i] == card.id)
                {
                    myCards[i] = -1;
                    break;
                }
            }
            playerProperties.Add("myCards", myCards);

            // GameManagerScript.Instance.GiveCards(card.cardIndex, card.cardIndex + 1, PhotonNetwork.LocalPlayer);
        }

        cardField.GetComponent<PFieldScript>().is_select = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    public void OnClickResult()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

        if ((TurnStates)PhotonNetwork.CurrentRoom.CustomProperties["turn_state"] == TurnStates.RESULTS)
        {
            // if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"])
            if (PhotonNetwork.IsMasterClient)
            {
                if (GameManagerScript.Instance.GameFinished())
                {
                    properties.Add("turn_state", TurnStates.FINISH);
                }
                else
                {
                    if (GameManagerScript.Instance.mainPlayer.ActorNumber == PhotonNetwork.CurrentRoom.PlayerCount)
                    {
                        Player player = PhotonNetwork.CurrentRoom.Players[1];
                        Debug.Log(GameManagerScript.Instance.mainPlayer.ActorNumber + "actor number");
                        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + "players count");
                        GameManagerScript.Instance.SetTurn(player);
                    }
                    else
                    {
                        GameManagerScript.Instance.SetTurn(GameManagerScript.Instance.mainPlayer.GetNext());
                    }
                    properties.Add("turn_state", TurnStates.MP_CHOSING);
                }
            }
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
}
