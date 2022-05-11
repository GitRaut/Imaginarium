using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginGame : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("BEGIN");
        SceneManager.LoadScene("Game");
    }
}
