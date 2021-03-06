using UnityEngine;
using UnityEngine.UI;


    public class PlayerStaminaBar : MonoBehaviour
    {
        /**
     * @author Martin Kings
     */
        public Slider staminaBar; // the slider 
        [SerializeField] private GlobalPlayerInfo globalPlayerInfo; // contains the global info of the current player

        private void Start()
        {
            staminaBar.maxValue = globalPlayerInfo.GetMaxStamina();
            staminaBar.value = globalPlayerInfo.GetStamina();
        }

        // Updates the value of the slider to the players current stamina (will be called upon when attackacking, sprinting etc)
        public void SetStamina(float stamina)
        {
            staminaBar.maxValue = globalPlayerInfo.GetMaxStamina();
            staminaBar.value = stamina;
        }
    }
