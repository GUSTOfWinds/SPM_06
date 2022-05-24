using ItemNamespace;
using UnityEngine;


public class SceneSwitch : MonoBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private Triggertooltip triggerTooltip;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }
        
        if (triggerTooltip.GetKeyStatus())
        {
            other.GetComponent<PlayerTeleport>().StartTeleport();
        }
    }

}