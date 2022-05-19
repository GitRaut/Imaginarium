using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Transform menu;
    public Transform lobby;
    public TMP_Text roomNameUI;
    public TMP_Text userListUI;
    private string roomName;

    public void OnClickReady()
    {
        if (PhotonNetwork.IsMasterClient)
        {
           if (AllReady() || PhotonNetwork.CurrentRoom.PlayerCount == 1)
           {
               Hashtable properties = new Hashtable();
               properties["GameStarted"] = true;
               PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
           } 
        }else{
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("isLobbyReady") || !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isLobbyReady"])
            {
                Hashtable properties = new Hashtable();
                properties["isLobbyReady"] = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
            }
        }
    }

    public bool AllReady(){
        bool allReady = true;
        foreach (Player player in PhotonNetwork.PlayerList){
            if (player != PhotonNetwork.MasterClient)
            {
                if (!player.CustomProperties.ContainsKey("isLobbyReady") || !(bool)player.CustomProperties["isLobbyReady"])
                {
                    allReady = false;
                }
            }
        }

        return allReady;
    }

    public void OnClickExit()
    {
        // this.SetRoomName(null);
        PhotonNetwork.LeaveRoom();
    }

    public void SetRoomName(string _roomName){
        this.roomName = _roomName;
        roomNameUI.SetText(this.roomName);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        base.OnPlayerEnteredRoom(player);
        UpdateInterface();
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        base.OnPlayerEnteredRoom(player);
        UpdateInterface();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        lobby.gameObject.SetActive(true);
        menu.gameObject.SetActive(false);
        UpdateInterface();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("isLobbyReady"))
        {
            UpdateInterface();
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("GameStarted") && (bool)propertiesThatChanged["GameStarted"])
        {
            SceneManager.LoadScene("Game");
        }
    }


    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        lobby.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
    }

    private void UpdateInterface() {
        string playerList = "";
 
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerList += " - " + player.NickName;
            if (player.CustomProperties.ContainsKey("isLobbyReady") && (bool)player.CustomProperties["isLobbyReady"])
            {
                playerList += " (Готов)";
            }
            playerList += "\n\n";
        }

        userListUI.text = playerList;
        roomNameUI.text = PhotonNetwork.CurrentRoom.Name;
    }
}
