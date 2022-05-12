using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField userNameInput;
    public TMP_InputField roomNameInput;
    public LobbyManager lobbyManager;

    public void OnClickJoin()
    {
        string roomName = roomNameInput.text;
        string userName = userNameInput.text;
        
        if (roomName.Length > 0 && userName.Length > 0)
        {
            Debug.Log("JoinRoom");
            PhotonNetwork.NickName = userName;
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public void OnClickCreate()
    {
        string roomName = roomNameInput.text;
        string userName = userNameInput.text;
        
        if (roomName.Length > 0 && userName.Length > 0)
        {
            Debug.Log("CREATE");
            PhotonNetwork.NickName = userName;
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Joining failed: " + message);
    }

    public void OnClickQuit()
    {
        Debug.Log("QUIT");
    }
}
