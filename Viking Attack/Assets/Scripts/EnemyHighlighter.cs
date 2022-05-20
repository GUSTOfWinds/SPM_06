using UnityEngine;
using Mirror;


public class EnemyHighlighter : NetworkBehaviour
{
    private Camera mainCamera;
    private Material material;
    private GameObject rayCastPosition;

    public void Start()
    {
        rayCastPosition = gameObject.transform.Find("rayCastPosition").gameObject;
        mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
    }

    public void Update()
    {
        if (!isLocalPlayer) return;
        //Sends a raycast to check for colliders in the Enemy layer
        Collider[] hits = Physics.OverlapSphere(rayCastPosition.transform.position, gameObject.GetComponent<PlayerItemUsageController>().itemBase.GetRange, LayerMask.GetMask("Enemy"));
        if (hits.Length > 0)
        {
            Collider hit = hits[0];
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