using UnityEngine;
using Mirror;
using TMPro;

public class MovePlayerOnMap : NetworkBehaviour
{
    private GameObject player;
    private RectTransform rectTransform;
    private TextMeshProUGUI text;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if(!isLocalPlayer)
            if(player != null)
            {
                rectTransform.anchoredPosition3D = new Vector3(player.gameObject.transform.position.x,player.gameObject.transform.position.z,-1)*3f;
            }   
            else
            {
                player = GameObject.FindGameObjectWithTag("Player");
                if(player != null)
                    text.text = (player.GetComponent<GlobalPlayerInfo>().GetName());
            }
                
    }
}
