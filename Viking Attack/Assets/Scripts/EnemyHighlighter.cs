using UnityEngine;
using Mirror;


public class EnemyHighlighter : NetworkBehaviour
{
    private Camera mainCamera;
    private RaycastHit hit;
    private Material material;

    public void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
    }

    public void Update()
    {
        if (!isLocalPlayer) return;
        //Sends a raycast to check for colliders in the Enemy layer
        if (Physics.SphereCast(mainCamera.transform.position, 2f, mainCamera.transform.forward, out hit, 10,
                LayerMask.GetMask("Enemy")))
        {
            material = hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
            material.SetFloat("Vector1_63645fd0daa5462ea5528c1ac3a77c0b", 1f);
        }
        else if (material != null)
        {
            //Set text to nothing
            material.SetFloat("Vector1_63645fd0daa5462ea5528c1ac3a77c0b", 0f);
        }
    }
}