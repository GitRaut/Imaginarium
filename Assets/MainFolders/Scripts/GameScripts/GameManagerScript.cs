using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameManagerScript : MonoBehaviourPun, IPunObservable
{
    public string asoc;
    public List<Sprite> remaining_cards;

    private void Update()
    {
        if (photonView && photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            Debug.Log(asoc);
        }
    }

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(asoc);
        }
        else
        {
            // Network player, receive data
            this.asoc = (string)stream.ReceiveNext();
        }
    }

    #endregion
}
