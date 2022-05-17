using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PButtonsScript : MonoBehaviour
{
    public GameManagerScript gameManager;

    public void OnClickPChoosing()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

        if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] && (TurnStates)PhotonNetwork.CurrentRoom.CustomProperties["turn_state"] == TurnStates.P_CHOSING)
        {
            properties.Add("turn_state", TurnStates.VOTING);
            playerProperties.Add("isReady", true);
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
