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
                playerProperties.Add("score", 0);
                // playerProperties.Add("voteCount", 0);
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

        if (changedProps.ContainsKey("score"))
        {
            UpdateResultsScreen();
        }
    }

    private void UpdateResultsScreen(){
        Transform text = resultScreen.transform.Find("UserList");
        TMP_Text textUi = text.GetComponent<TMP_Text>();
        if (textUi)
        {
            string scoreText = "";
 
            foreach (var player in PhotonNetwork.PlayerList)
            {
                int score = 0;
                if (player.CustomProperties.ContainsKey("score")) score = (int)player.CustomProperties["score"];
                int lastAdd = 0;
                if (player.CustomProperties.ContainsKey("lastAdd")) lastAdd = (int)player.CustomProperties["lastAdd"];
                
                scoreText += " - " + player.NickName + " " + score + "(+" + lastAdd + ")" + "\n\n";
            }

            textUi.text = scoreText;
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
                        clearTurnData();
                        resultScreen.gameObject.SetActive(false);
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
                    CalculateResults();
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

    private Player MainPlayer(){
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("myTurn") && (bool)player.CustomProperties["myTurn"])
            {
                return player;
            }
        }
        return null;
    }

    private void CalculateResults(){
        Hashtable data = new Hashtable();
        bool allVotes = true;
        bool hasVotes = false;
        Player mainPlayer = MainPlayer(); 

        Hashtable mainPlayerProperties = mainPlayer.CustomProperties;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p != mainPlayer)
            {
                if (mainPlayerProperties.ContainsKey("vote_" + p.ActorNumber.ToString()))
                {
                    hasVotes = true;
                }else{
                    allVotes = false;
                }
            }
        }

        if (allVotes)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p != mainPlayer)
                {
                    int initialScore = 0;
                    if (p.CustomProperties.ContainsKey("score")) initialScore = (int)p.CustomProperties["score"];
                    Hashtable properties = new Hashtable();
                    properties.Add("score", initialScore + 3);
                    properties.Add("lastAdd", 3);
                    p.SetCustomProperties(properties);
                }
            }
        }else if(!allVotes && hasVotes){
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                int initialScore = 0;
                if (p.CustomProperties.ContainsKey("score")) initialScore = (int)p.CustomProperties["score"];

                int addScore = 0;
                if (p == mainPlayer) addScore = 3;

                foreach (Player anotherPlayer in PhotonNetwork.PlayerList)
                {
                    if (anotherPlayer != mainPlayer && anotherPlayer != p)
                    {
                        if (
                            p.CustomProperties.ContainsKey("vote_" + anotherPlayer.ActorNumber.ToString()) && 
                            (bool)p.CustomProperties["vote_" + anotherPlayer.ActorNumber.ToString()]
                        )
                        {
                            addScore += 1;
                        }
                    }
                }

                if (addScore > 0)
                {
                    Hashtable properties = new Hashtable();
                    properties.Add("score", initialScore + addScore);
                    properties.Add("lastAdd", addScore);
                    p.SetCustomProperties(properties);
                }
            }
        }else if(!allVotes && !hasVotes){
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                int initialScore = 0;
                if (p.CustomProperties.ContainsKey("score")) initialScore = (int)p.CustomProperties["score"];

                int addScore = 2;

                foreach (Player anotherPlayer in PhotonNetwork.PlayerList)
                {
                    if (anotherPlayer != mainPlayer && anotherPlayer != p)
                    {
                        if (
                            p.CustomProperties.ContainsKey("vote_" + anotherPlayer.ActorNumber.ToString()) && 
                            (bool)p.CustomProperties["vote_" + anotherPlayer.ActorNumber.ToString()]
                        )
                        {
                            addScore += 1;
                        }
                    }
                }

                if (addScore > 0)
                {
                    Hashtable properties = new Hashtable();
                    properties.Add("score", initialScore + addScore);
                    properties.Add("lastAdd", addScore);
                    p.SetCustomProperties(properties);
                }
            }
        }

        // foreach (Player player in PhotonNetwork.PlayerList)
        // {
        //     int totalVotes = 0;
        //     Hashtable playerProperties = player.CustomProperties;

        //     foreach (Player p in PhotonNetwork.PlayerList)
        //     {
        //         if (playerProperties.ContainsKey("vote_" + p.ActorNumber.ToString()))
        //         {
        //             totalVotes += 1;
        //         }
        //     }

        //     data[player.ActorNumber.ToString()] = totalVotes;
        // }
    }

    private void clearTurnData(){
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Hashtable playerProperties = new Hashtable();
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (playerProperties.ContainsKey("vote_" + p.ActorNumber.ToString()))
                {
                    playerProperties.Add("vote_" + p.ActorNumber.ToString(), null);
                }
            }
            player.SetCustomProperties(playerProperties);
        }
    }
}
