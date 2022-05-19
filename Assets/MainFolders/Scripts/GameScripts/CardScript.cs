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
    public int cardIndex;

    private void Start()
    {
        is_used = false;
        main_camera = Camera.main;
        def_parent = transform.parent;
        cardIndex = transform.GetSiblingIndex();
    }

    public void ShowCardInfo()
    {
        int[] cards = (int[])PhotonNetwork.LocalPlayer.CustomProperties["myCards"];
        if (cards[cardIndex] != -1)
        {
            gameObject.SetActive(true);
            image.sprite = GameManagerScript.Instance.allCards[cards[cardIndex]];
            id = cards[cardIndex];
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
