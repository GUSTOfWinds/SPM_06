using UnityEngine;
using Mirror;
using TMPro;

public class MovePlayerOnMap : MonoBehaviour
{
    /*
     *  @Author Love Strignert - lost9373
    */
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
        rectTransform.anchoredPosition3D = new Vector3(player.gameObject.transform.position.x,player.gameObject.transform.position.z,-1)*3f;
    }

    public void SetPlayer(GameObject player) => this.player = player;
}
