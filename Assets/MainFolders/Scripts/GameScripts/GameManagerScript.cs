using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManagerScript : MonoBehaviourPunCallbacks
{
    public string asoc;
    public List<Sprite> remaining_cards;
    public TMP_Text asoc_field;
    public List<Sprite> selected_cards;
    private int player_index;

    private void Awake()
    {
        player_index = 0;
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        asoc = (string)propertiesThatChanged["asoc"];
        asoc_field.text = asoc;
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if (stream.IsWriting)
    //     {
    //         stream.SendNext(asoc);
    //     }
    //     else
    //     {
    //         asoc = (string)stream.ReceiveNext();
    //     }
    // }

    // private void Update()
    // {
    //     if(asoc_field.text != asoc)
    //     {
    //         asoc_field.text = asoc;
    //     }
    // }
}
