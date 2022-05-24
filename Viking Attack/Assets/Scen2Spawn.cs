using UnityEngine;

public class Scen2Spawn : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = this.gameObject.transform.position;
    }
}
