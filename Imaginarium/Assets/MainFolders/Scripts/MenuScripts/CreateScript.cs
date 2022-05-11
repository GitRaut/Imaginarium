using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateScript : MonoBehaviour
{
    public Transform lobby;

    public void OnClick()
    {
        Debug.Log("CREATE");
        lobby.gameObject.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }
}