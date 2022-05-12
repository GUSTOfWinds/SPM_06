using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerOnMap : MonoBehaviour
{
    private GameObject player;
    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void Update()
    {
        if(player != null)
            rectTransform.anchoredPosition3D = new Vector3(player.gameObject.transform.position.x,player.gameObject.transform.position.z,-1)*3f;
        else
            player = GameObject.FindGameObjectWithTag("Player");
    }
}
