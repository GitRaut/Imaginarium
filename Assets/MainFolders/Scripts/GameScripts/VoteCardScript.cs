using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class VoteCardScript : MonoBehaviour
{
    public Image image;
    public Transform cardsField;
    private GameManagerScript gameManager;

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        cardsField = GameObject.Find("Cards").transform;
    }

    public void ShowCardInfo()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        int index;
        int[] cards = (int[])PhotonNetwork.CurrentRoom.CustomProperties["selected_cards"];
        do
        {
            index = Random.Range(0, cards.Length - 1);
        }
        while (index == 999);
        image.sprite = GameManagerScript.Instance.allCards[cards[index]];
        cards[index] = 999;
        properties.Add("selected_cards", cards);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
}
