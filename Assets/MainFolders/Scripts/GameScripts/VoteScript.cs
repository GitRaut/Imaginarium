using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class VoteScript : MonoBehaviour
{
    public Transform cardTransform;

    public void OnClickVote()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

        if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] && (TurnStates)PhotonNetwork.CurrentRoom.CustomProperties["turn_state"] == TurnStates.VOTING)
        {
            VoteCardScript card = cardTransform.GetComponent<VoteCardScript>();
            foreach (Player listPlayer in PhotonNetwork.PlayerList)
            {
                if (card.id == (int)listPlayer.CustomProperties["selectedCard"])
                {
                    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
                    // int voteCount = (int)listPlayer.CustomProperties["voteCount"];
                    // playerProperties.Add("voteCount", voteCount + 1);
                    playerProperties.Add("vote_" + PhotonNetwork.LocalPlayer.ActorNumber.ToString(), true);
                    listPlayer.SetCustomProperties(playerProperties);
                }
            }
            properties.Add("turn_state", TurnStates.RESULTS);
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
}
