using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public List<Sprite> remaining_cards;
    public TMP_Text asoc_field;
    public List<Sprite> selected_cards;

    [Header("Screens")]
    public Transform mpChooseScreen;
    public Transform waitingScreen;
    public Transform pChooseScreen;
    public Transform voteScreen;
    public Transform resultScreen;

    public struct Card
    {
        string id;
        Sprite image;

        public Card(string id)
        {
            this.id = id;
            image = Resources.Load<Sprite>("MainFolders/Textures/Cards/" + id);
        }
    }

    private void LoadCards()
    {

    }

    private void Start()
    {
        this.StartGame();
    }

    private void StartGame(){
        if (PhotonNetwork.IsMasterClient)
        {
            this.SetTurn(PhotonNetwork.LocalPlayer);

            Hashtable properties = new Hashtable();
            properties.Add("remaining_cards", remaining_cards);
            properties.Add("selected_cards", selected_cards);
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

    public void GiveCards(int col, Player player)
    {

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer.IsLocal)
        {
            Debug.LogFormat("IS MY TURN {0}", (bool)changedProps["myTurn"]);
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
