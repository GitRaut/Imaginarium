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
    RESULTS = 3,
    FINISH = 4
}

public class GameManagerScript : MonoBehaviourPunCallbacks
{
    public int TOTAL_CARDS_AMOUNT = 6;
    public static GameManagerScript Instance;
    public string asoc;
    public TMP_Text asoc_field;
    public Sprite[] allCards;
    public List<int> remainingCards;
    public TMP_Text winnerName;
    // public List< remainingCards;
    // public int[] remainingCards;

    [Header("Screens")]
    public Transform mpChooseScreen;
    public Transform waitingScreen;
    public Transform pChooseScreen;
    public Transform voteScreen;
    public Transform resultScreen;
    public Transform finishScreen;

    public Transform cardPrefab;
    public Player mainPlayer;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Awake()
    {
        int col = (allCards.Length / PhotonNetwork.CurrentRoom.PlayerCount) * PhotonNetwork.CurrentRoom.PlayerCount;
        for (int i = 0; i < col; i++)
        {
            remainingCards.Add(i);
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
                // bool ready = false;
                // playerProperties.Add("isReady", ready);
                Hashtable playerProperties = new Hashtable();
                playerProperties.Add("score", 0);
                playerProperties.Add("selectedCard", -1);
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            }

            GiveCardsNew();
            // foreach (Player listPlayer in PhotonNetwork.PlayerList)
            // {
            //     // GiveCards(0, 6, listPlayer);
            //     GiveCardsNew(0, 6, listPlayer);
            // }

            Hashtable properties = new Hashtable();
            properties.Add("selected_cards", new int[PhotonNetwork.CurrentRoom.PlayerCount]);
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

    // public void GiveCards(int begIndex, int endIndex, Player player)
    // {
    //     int id = 999;
    //     int[] cards = (int[])player.CustomProperties["myCards"];
    //     if (cards == null) cards = new int[TOTAL_CARDS_AMOUNT];

    //     if (cards != null && remainingCards != null)
    //     {
    //         for (int i = begIndex; i < endIndex; i++)
    //         {
    //             do
    //             {
    //                 id = Random.Range(0, remainingCards.Length - 1);
    //             }
    //             while (remainingCards[id] == 999);

    //             cards[i] = remainingCards[id];
    //             remainingCards[id] = 999;
    //         }
    //     }

    //     Hashtable properties = new Hashtable();
    //     properties.Add("myCards", cards);
    //     player.SetCustomProperties(properties);
    // }

    public void GiveCardsNew(){
        bool isGived = true;

        Hashtable data = new Hashtable();
        
        while (remainingCards.Count > 0 && isGived)
        {
            isGived = false;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                int[] cards = null;
                if (!data.ContainsKey(player.ActorNumber.ToString()))
                {
                    cards = (int[])player.CustomProperties["myCards"];
                    if (cards == null) {
                        cards = new int[TOTAL_CARDS_AMOUNT];
                        for (int i = 0; i < cards.Length; i++)
                        {
                            cards[i] = -1;
                        }
                    }
                    data[player.ActorNumber.ToString()] = cards;
                }else{
                    cards = (int[])data[player.ActorNumber.ToString()];
                }

                for (int i = 0; i < cards.Length; i++)
                {
                    int cardIndex = cards[i];
                    if (cardIndex == -1)
                    {
                        if (remainingCards.Count > 0)
                        {
                            int index = Random.Range(0, remainingCards.Count - 1);
                            cards[i] = remainingCards[index];
                            remainingCards.Remove(remainingCards[index]);
                            isGived = true;

                            data[player.ActorNumber.ToString()] = cards;
                        }
                        break;
                    }
                }
            }
        }

        foreach (Player player in PhotonNetwork.PlayerList){
            if (data.ContainsKey(player.ActorNumber.ToString()))
            {
                Hashtable userProperties = new Hashtable();
                userProperties.Add("myCards", (int[])data[player.ActorNumber.ToString()]);
                player.SetCustomProperties(userProperties);

            }
        }

        Debug.Log(string.Join(",", remainingCards));
        Debug.Log(remainingCards.Count);
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

        if (changedProps.ContainsKey("myTurn") && (bool)changedProps["myTurn"]){
            mainPlayer = targetPlayer;
        }

        if (changedProps.ContainsKey("selectedCard"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                bool allSelected = true;
                foreach (Player player in PhotonNetwork.PlayerList){
                    if (player != mainPlayer)
                    {
                        Hashtable properties = player.CustomProperties;
                        Debug.Log((int)properties["selectedCard"]);
                        if (!properties.ContainsKey("selectedCard") || (int)properties["selectedCard"] == -1)
                        {
                            allSelected = false;
                        }
                    }
                }

                if (allSelected && (TurnStates)PhotonNetwork.CurrentRoom.CustomProperties["turn_state"] == TurnStates.P_CHOSING)
                {
                    Hashtable roomProperties = new Hashtable();
                    roomProperties.Add("turn_state", TurnStates.VOTING);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                }
            }
        }

        if (changedProps.ContainsKey("voted"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                bool allVoted = true;
                foreach (Player player in PhotonNetwork.PlayerList){
                    if (player != mainPlayer)
                    {
                        Hashtable properties = player.CustomProperties;
                        if (!properties.ContainsKey("voted") || !(bool)properties["voted"])
                        {
                            allVoted = false;
                        }
                    }
                }

                if (allVoted && (TurnStates)PhotonNetwork.CurrentRoom.CustomProperties["turn_state"] == TurnStates.VOTING)
                {
                    Hashtable roomProperties = new Hashtable();
                    roomProperties.Add("turn_state", TurnStates.RESULTS);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                }
            }
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
                        resultScreen.gameObject.SetActive(false);
                        mpChooseScreen.gameObject.SetActive(true);
                    }
                    else{
                        resultScreen.gameObject.SetActive(false);
                        mpChooseScreen.gameObject.SetActive(false);
                        waitingScreen.gameObject.SetActive(true);
                    }
                    break;
                case TurnStates.P_CHOSING:
                    Transform field = pChooseScreen.Find("SetField");
                    field.GetComponent<PFieldScript>().is_select = false;
                    if ( !(bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] )
                    {
                        waitingScreen.gameObject.SetActive(false);
                        pChooseScreen.gameObject.SetActive(true);
                    }
                    else{
                        mpChooseScreen.gameObject.SetActive(false);
                        waitingScreen.gameObject.SetActive(true);
                    }
                    break;
                case TurnStates.VOTING:
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
                    if (PhotonNetwork.IsMasterClient) {
                        CalculateResults();
                        clearTurnData();
                        GiveCardsNew();
                    }
                    voteScreen.gameObject.SetActive(false);
                    resultScreen.gameObject.SetActive(true);
                    break;
                case TurnStates.FINISH:
                    SetWinner();
                    resultScreen.gameObject.SetActive(false);
                    finishScreen.gameObject.SetActive(true);
                    break;
            }
        }
        if (propertiesThatChanged.ContainsKey("asoc")){
            asoc = (string)propertiesThatChanged["asoc"];
            asoc_field.text = asoc;
        }
        if (propertiesThatChanged.ContainsKey("selected_cards"))
        {
            Transform cardsField = voteScreen.Find("Cards");
            foreach(Transform cardTransform in cardsField)
            {
                VoteCardScript card = cardTransform.GetComponent<VoteCardScript>();
                card.ShowCardInfo();
            }
        }
    }

    private void SetWinner()
    {
        int max = 0;
        foreach(Player listPlayer in PhotonNetwork.PlayerList)
        {
            int score = (int)listPlayer.CustomProperties["score"];
            if(score > max)
            {
                max = score;
                winnerName.text = listPlayer.NickName;
            }
        }
    }

    private void CalculateResults(){
        bool allVotes = true;
        bool hasVotes = false;

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
            playerProperties.Add("voted", null);
            playerProperties.Add("selectedCard", -1);
            player.SetCustomProperties(playerProperties);
        }
    }

    public bool GameFinished(){
        bool playersHasCards = false;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int[] myCards = (int[])PhotonNetwork.LocalPlayer.CustomProperties["myCards"];
            foreach (int card in myCards)
            {
                if (card != -1)
                {
                    playersHasCards = true;
                    break;
                }
            }
            if (playersHasCards)
            {
                break;
            }
        }

        if (remainingCards.Count == 0 && !playersHasCards)
        {
            return true;
        }
        return false;
    }
}
