using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyScript : MonoBehaviour
{
    public Transform next_screen;

    public void OnClick()
    {
        Debug.Log("DONE");
        next_screen.gameObject.SetActive(true);
        transform.gameObject.SetActive(false);
    }
}
