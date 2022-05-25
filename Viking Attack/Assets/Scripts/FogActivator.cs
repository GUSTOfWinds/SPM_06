using UnityEngine;
/**
 * @author Victor Wikner
 */
public class FogActivator : MonoBehaviour
{
    
    //this script is made to make it simple when editing a scene so the fog.object used isn't neeeded to be active. This object gets destroyed as it's run
    [SerializeField] private GameObject fog;
    [SerializeField] private GameObject destroyObject;

    private void Awake()
    {
        fog.SetActive(true);
    }

    private void Start()
    {
        GameObject.Destroy(destroyObject);
    }
}
