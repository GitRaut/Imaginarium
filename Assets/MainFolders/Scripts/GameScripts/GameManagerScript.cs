using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using Photon.Realtime;

enum TurnStates
{
    MP_CHOSING = 0,
    P_CHOSING = 1,
    VOTING = 2,
    RESULTS = 3
}

public class GameManagerScript : MonoBehaviourPunCallbacks
{
    public string asoc;
    public TMP_Text asoc_field;
    public Sprite[] allCards;
    public int[] remainingCards;
    public int[] selectedCards;

    [Header("Screens")]
    public Transform mpChooseScreen;
    public Transform waitingScreen;
    public Transform pChooseScreen;
    public Transform voteScreen;
    public Transform resultScreen;

    private void Awake()
    {
        selectedCards = new int[PhotonNetwork.CountOfPlayers];
        remainingCards = new int[50];
        for(int i = 0; i < remainingCards.Length; i++)
        {
            remainingCards[i] = i;
        }
        this.StartGame();
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.SetTurn(PhotonNetwork.LocalPlayer);

            foreach (Player listPlayer in PhotonNetwork.PlayerList)
            {
                bool ready = false;
                Hashtable playerProperties = new Hashtable();
                playerProperties.Add("isReady", ready);
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            }
            GiveCards(0);

            Hashtable properties = new Hashtable();
            properties.Add("remaining_cards", remainingCards);
            properties.Add("selected_cards", selectedCards);
            properties.Add("turn_state", TurnStates.MP_CHOSING);
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
    }

    public void SetTurn(Player player){
        foreach (Player listPlayer in PhotonNetwork.PlayerList)
        {
            bool turnPlayer = false;
            if (listPlayer == player)
            {
                turnPlayer = true;
            }
            Hashtable properties = new Hashtable();
            properties.Add("myTurn", turnPlayer);
            listPlayer.SetCustomProperties(properties);
        }
    }

    public void GiveCards(int begIndex)
    {
        foreach (Player listPlayer in PhotonNetwork.PlayerList)
        {
            Debug.Log(remainingCards == null);

            int id = 999;
            int[] cards = (int[])listPlayer.CustomProperties["myCards"];
            if (cards == null) cards = new int[6];

            if (cards != null && remainingCards != null)
            { 
                for (int i = begIndex; i < cards.Length; i++)
                {
                    do
                    {
                        id = Random.Range(0, remainingCards.Length - 1);
                    }
                    while (remainingCards[id] == 999);

                    cards[i] = remainingCards[id];
                    remainingCards[id] = 999;
                }

                Hashtable properties = new Hashtable();
                properties.Add("myCards", cards);
                listPlayer.SetCustomProperties(properties);
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer.IsLocal)
        {
            if (changedProps.ContainsKey("myCards"))
            {
                Transform hand = mpChooseScreen.Find("Hand");
                foreach (Transform cardTransform in hand)
                {
                    CardScript card = cardTransform.GetComponent<CardScript>();
                    card.ShowCardInfo();
                    // int[] myCards = (int[])changedProps["myCards"];
                    // Debug.Log(string.Join(",", myCards));
                }
            }
        }

        if (changedProps.ContainsKey("isReady"))
        {

        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("turn_state"))
        {
            switch ((TurnStates)propertiesThatChanged["turn_state"])
            {
                case TurnStates.MP_CHOSING:
                    if ( (bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] )
                    {
                        Debug.Log("CHOSING_SCREEN");
                        mpChooseScreen.gameObject.SetActive(true);
                    }
                    else{
                        Debug.Log("WAITING_SCREEN");
                        mpChooseScreen.gameObject.SetActive(false);
                        waitingScreen.gameObject.SetActive(true);
                    }
                    break;
                case TurnStates.P_CHOSING:
                    if ( !(bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] )
                    {
                        Debug.Log("CHOSING_SCREEN");
                        waitingScreen.gameObject.SetActive(false);
                        pChooseScreen.gameObject.SetActive(true);
                    }
                    else{
                        Debug.Log("WAITING_SCREEN");
                        mpChooseScreen.gameObject.SetActive(false);
                        waitingScreen.gameObject.SetActive(true);
                    }
                    break;
                case TurnStates.VOTING:
                    Debug.Log("VOTING_SCREEN");
                    waitingScreen.gameObject.SetActive(false);
                    voteScreen.gameObject.SetActive(true);
                    break;
                case TurnStates.RESULTS:
                    Debug.Log("RESULT_SCREEN");
                    voteScreen.gameObject.SetActive(false);
                    resultScreen.gameObject.SetActive(true);
                    break;
            }
        }
        if (propertiesThatChanged.ContainsKey("asoc")){
            asoc = (string)propertiesThatChanged["asoc"];
            asoc_field.text = asoc;
        }
    }
}
