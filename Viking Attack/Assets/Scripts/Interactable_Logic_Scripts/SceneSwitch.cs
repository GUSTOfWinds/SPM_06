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
            Debug.Log("Key has been picked up. Will portal player now");
            
            
            // Add teleports the player here
        }
    }

}