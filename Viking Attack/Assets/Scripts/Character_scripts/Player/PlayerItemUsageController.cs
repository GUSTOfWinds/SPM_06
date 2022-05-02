using DefaultNamespace;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerItemUsageController : NetworkBehaviour
{
    [SerializeField] private ItemBase itemBase; // Will need to be updated if another item is being used.
    private Camera mainCamera = null;
    private GlobalPlayerInfo globalPlayerInfo;
    private float attackStaminaCost = 20f;
    RaycastHit hit;


    public void Start()
    {
        if (!isLocalPlayer) return;
        mainCamera = GameObject.FindGameObjectWithTag("CameraMain").GetComponent<Camera>();
        globalPlayerInfo = gameObject.GetComponent<GlobalPlayerInfo>();
    }

    // WHO TO BLAME: Martin Kings
    public void OnAttack(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            if (!isLocalPlayer) return;
            if (itemBase.GetType() == ItemBase.Type.Food) // checks to see if the itembase is food in the playerhand
            {
                // eat, to be implemented
            }
            else if // ATTACK HERE
                (itemBase.GetType() ==
                 ItemBase.Type.Weapon) // checks to see if the itembase is a weapon in the playerhand
            {
                if (Physics.SphereCast(mainCamera.transform.position, 1f,
                        mainCamera.transform.forward, out hit, itemBase.GetRange(),
                        LayerMask.GetMask("Enemy")))
                {
                    // Checks if the player has enough stamina to attack, will then attack.
                    if (globalPlayerInfo.GetStamina() > attackStaminaCost)
                    {
                        globalPlayerInfo.UpdateStamina(-attackStaminaCost);
                        hit.collider.gameObject.GetComponent<EnemyVitalController>()
                            .CmdUpdateHealth(-itemBase.GetDamage());
                        // ADD SWING ANIMATION HERE
                    }
                }
            }
            else if
                (itemBase.GetType() == ItemBase.Type.Tool) // checks to see if the itembase is a tool in the playerhand
            {
                // do tool stuff, to be implemented
            }
            else // ff the itembase is key in the playerhand
            {
                // do key stuff, to be implemented
            }
        }
    }
}