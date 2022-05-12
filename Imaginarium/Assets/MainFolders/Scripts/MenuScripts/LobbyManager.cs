using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Transform menu;
    public Transform lobby;
    public TMP_Text roomNameUI;
    public TMP_Text userListUI;
    private string roomName;
    private List<string> userNames;

    public void OnClickReady()
    {
        Debug.Log("Ready");
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
        userNames.Add(player.UserId);
        UpdateInterface();
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        base.OnPlayerEnteredRoom(player);
        userNames.Remove(player.UserId);
        UpdateInterface();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        lobby.gameObject.SetActive(true);
        menu.gameObject.SetActive(false);
        UpdateInterface();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        lobby.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
    }

    private void UpdateInterface() {
         string playerList = "";
 
        foreach (var player in PhotonNetwork.PlayerList)
        {
            playerList += " - " + player.NickName + "\n";
        }

        userListUI.text = playerList;
        roomNameUI.text = PhotonNetwork.CurrentRoom.Name;
    }
}
