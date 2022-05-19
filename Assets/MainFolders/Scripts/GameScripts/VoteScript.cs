using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class VoteScript : MonoBehaviour
{
    public Transform cardsField;
    public Transform cardPrefab;

    private void Start()
    {
        cardsField = GameObject.Find("Cards").transform;
        for(int i = 0; i < PhotonNetwork.CountOfPlayers; i++)
        {
            Transform card = Instantiate(cardPrefab, parent: cardsField);
        }
    }
}
