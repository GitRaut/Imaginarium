using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class CardScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 offset;
    private Camera main_camera;
    public Transform def_parent;
    public Image image;
    public bool is_used;
    //private GameManagerScript gameManager;
    public int id;

    private void Start()
    {
        is_used = false;
        main_camera = Camera.main;
        def_parent = transform.parent;
    }

    public void ShowCardInfo()
    {
        int index = transform.GetSiblingIndex();
        Debug.LogFormat("INDEX {0}", index);
        int[] cards = (int[])PhotonNetwork.LocalPlayer.CustomProperties["myCards"];
        if (cards[index] != -1)
        {
            gameObject.SetActive(true);
            image.sprite = GameManagerScript.Instance.allCards[cards[index]];
            id = cards[index];
        }else{
            gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!is_used)
        {
            offset = transform.position - main_camera.ScreenToWorldPoint(eventData.position);
            transform.SetParent(def_parent.parent);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!is_used) 
        {
            Vector2 new_pos = main_camera.ScreenToWorldPoint(eventData.position);
            transform.position = new_pos + offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
            transform.SetParent(def_parent);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
