using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class VoteCardScript : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Transform cardsField;
    public VoteScript voteTransfrom;
    private GameManagerScript gameManager;
    public int id;

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        cardsField = GameObject.Find("Cards").transform;
    }

    public void ShowCardInfo()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        int index = transform.GetSiblingIndex();
        int[] cards = (int[])PhotonNetwork.LocalPlayer.CustomProperties["selected_cards"];
        image.sprite = GameManagerScript.Instance.allCards[cards[index]];
        id = cards[index];
        Debug.Log(string.Join(",", cards));
        Debug.Log(index);
        Debug.Log(cards[index]);
        /*do
        {
            index = Random.Range(0, cards.Length - 1);
        }
        while (cards[index] == 999);*/
        //cards[index] = 999;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        VoteCardScript card = transform.GetComponent<VoteCardScript>();
        if (card.id != (int)PhotonNetwork.LocalPlayer.CustomProperties["selectedCard"])
        {
            Debug.Log("green");
            image.color = new Color(139, 255, 136, 255);
            voteTransfrom.cardTransform = this.transform;
        }
    }
}
