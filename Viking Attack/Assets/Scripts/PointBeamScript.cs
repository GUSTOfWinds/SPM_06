using UnityEngine;

public class PointBeamScript : MonoBehaviour
{
    private GameObject player;
    private MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
            if(Vector3.Distance(player.transform.position, gameObject.transform.position) < 40)
            {
                meshRenderer.enabled = false;
            }else
            {
                meshRenderer.enabled = true;
            }
        else
            player = GameObject.FindGameObjectWithTag("Player");
    }
}
