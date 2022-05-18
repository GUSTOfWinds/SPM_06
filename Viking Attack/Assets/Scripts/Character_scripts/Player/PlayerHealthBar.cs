using UnityEngine;
using UnityEngine.UI;

namespace Character_scripts.Player
{
    public class PlayerHealthBar : MonoBehaviour
    {
        /**
     * @author Martin Kings
     */
        public Slider healthBar; // the slider 
        [SerializeField] private GlobalPlayerInfo globalPlayerInfo; // contains the global info of the current player

        private void Start()
        {
            healthBar.maxValue = globalPlayerInfo.GetMaxHealth();
            healthBar.value = globalPlayerInfo.GetHealth();
        }

        // Updates the value of the slider to the players current health (will be called upon when being attacked, healed etc)
        public void SetHealth(float health)
        {
            healthBar.maxValue = globalPlayerInfo.GetMaxHealth();
            healthBar.value = health;
        }
    }
}