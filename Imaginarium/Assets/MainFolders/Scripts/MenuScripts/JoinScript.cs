using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinScript : MonoBehaviour
{
    public Transform lobby;

    public void OnClick()
    {
        Debug.Log("JOIN");
        lobby.gameObject.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }
}
