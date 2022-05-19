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
    public static GameManagerScript Instance;
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

    public Transform cardPrefab;

    private void Start()
    {
        // ������, ��������� ������������� ����������
        if (Instance == null)
        { // ��������� ��������� ��� ������
            Instance = this; // ������ ������ �� ��������� �������
        }
    }

    private void Awake()
    {
        selectedCards = new int[PhotonNetwork.CurrentRoom.PlayerCount];
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
                playerProperties.Add("points", 0);
                playerProperties.Add("voteCount", 0);
                playerProperties.Add("selectedCard", 0);
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            }
            foreach (Player listPlayer in PhotonNetwork.PlayerList)
            {
                GiveCards(0, 6, listPlayer);
            }

            Hashtable properties = new Hashtable();
            properties.Add("remaining_cards", remainingCards);
            properties.Add("selected_cards", selectedCards);
            properties.Add("turn_state", TurnStates.MP_CHOSING);
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);

        }
        Transform cardsField = voteScreen.transform.Find("Cards").transform;
        for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            Transform card = Instantiate(cardPrefab, parent: cardsField);
            card.GetComponent<VoteCardScript>().voteTransfrom = voteScreen.transform.GetComponent<VoteScript>();
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

    public void GiveCards(int begIndex, int endIndex, Player player)
    {
        int id = 999;
        int[] cards = (int[])player.CustomProperties["myCards"];
        if (cards == null) cards = new int[6];

        if (cards != null && remainingCards != null)
        {
            for (int i = begIndex; i < endIndex; i++)
            {
                do
                {
                    id = Random.Range(0, remainingCards.Length - 1);
                }
                while (remainingCards[id] == 999);

                cards[i] = remainingCards[id];
                remainingCards[id] = 999;
            }
        }

        Hashtable properties = new Hashtable();
        properties.Add("myCards", cards);
        player.SetCustomProperties(properties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer.IsLocal)
        {
            if (changedProps.ContainsKey("myCards"))
            {
                List<Transform> hands = new List<Transform>
                {
                    mpChooseScreen.Find("Hand"),
                    waitingScreen.Find("Hand"),
                    pChooseScreen.Find("Hand")
                };
                foreach (Transform hand in hands)
                {
                    foreach(Transform cardTransform in hand){
                        CardScript card = cardTransform.GetComponent<CardScript>();
                        card.ShowCardInfo();
                    }
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
                        resultScreen.gameObject.SetActive(false);
                        mpChooseScreen.gameObject.SetActive(true);
                    }
                    else{
                        Debug.Log("WAITING_SCREEN");
                        resultScreen.gameObject.SetActive(false);
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
                    pChooseScreen.gameObject.SetActive(false);
                    waitingScreen.gameObject.SetActive(false);
                    voteScreen.gameObject.SetActive(true);
                    Transform voteButton = voteScreen.Find("Button");
                    if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"])
                    {
                        voteButton.gameObject.GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        voteButton.gameObject.GetComponent<Button>().interactable = true;
                    }
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
        if (propertiesThatChanged.ContainsKey("selected_cards"))
        {
            selectedCards = (int[])propertiesThatChanged["selected_cards"];

            Transform cardsField = voteScreen.Find("Cards");
            foreach(Transform cardTransform in cardsField)
            {
                Debug.Log("SHOW CARD INFO");
                VoteCardScript card = cardTransform.GetComponent<VoteCardScript>();
                card.ShowCardInfo();
            }
        }
    }
}
