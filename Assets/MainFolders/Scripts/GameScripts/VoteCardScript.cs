using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class VoteCardScript : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Outline outline;
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
        int[] cards = (int[])PhotonNetwork.CurrentRoom.CustomProperties["selected_cards"];
                
        image.sprite = GameManagerScript.Instance.allCards[cards[index]];
        id = cards[index];

        /*do
        {
            index = Random.Range(0, cards.Length - 1);
        }
        while (cards[index] == 999);*/
        //cards[index] = 999;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("myTurn") && (bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"])
            return;

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        VoteCardScript card = transform.GetComponent<VoteCardScript>();
        if (card.id != (int)PhotonNetwork.LocalPlayer.CustomProperties["selectedCard"])
        {
            foreach (Transform voteCard in transform.parent)
            {
                if (voteCard != transform)
                {
                    VoteCardScript scr = voteCard.GetComponent<VoteCardScript>();
                    scr.outline.effectDistance = new Vector2(0,0);
                }
            }
            outline.effectDistance = new Vector2(10,10);
            voteTransfrom.cardTransform = this.transform;
        }
    }
}
