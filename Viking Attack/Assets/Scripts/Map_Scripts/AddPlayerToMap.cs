using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddPlayerToMap : MonoBehaviour
{
    /*
     *  @Author Love Strignert - lost9373
    */
    private GameObject[] players;
    [SerializeField] private GameObject playerOnMap;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            GameObject temp = Instantiate(playerOnMap,transform);
            temp.GetComponent<TextMeshProUGUI>().text = player.GetComponent<GlobalPlayerInfo>().GetName();
            temp.GetComponent<MovePlayerOnMap>().SetPlayer(player);
        }   
    }
}
