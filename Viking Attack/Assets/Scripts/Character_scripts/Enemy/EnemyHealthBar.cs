using ItemNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;


namespace Character_scripts.Enemy
{
    public class EnemyHealthBar : MonoBehaviour
    {
        /**
     * @author Martin Kings
     */
        [SerializeField] private Slider healthBar; // the slider 

        [SerializeField] private GameObject healthSource; // the enemy gameobject
        private EnemyVitalController enemyVitalController;
        [SerializeField] private TMP_Text enemyLevelText;
        [SerializeField] private TMP_Text enemyName;
        [SerializeField] private Image enemyImage;


        private void Update()
        {
            SetHealth();
        }

        public void SetHealthSource(GameObject hs)
        {
            healthSource = hs;
            enemyVitalController = healthSource.GetComponent<EnemyVitalController>();
        }

        // Updates the health number of the slider
        public void SetHealth()
        {
            healthBar.maxValue = enemyVitalController.GetMaxHealth();
            healthBar.value = enemyVitalController.GetCurrentHealth();
        }

        public void Setup(GameObject enemy)
        {
            // TODO: Add image of enemy from sprite in character base

            // Sets the name text to the enemy name
            enemyName.text = enemy.GetComponent<EnemyInfo>().GetName();

            // Sets the level text to the enemy name
            enemyLevelText.text = "Level: " + enemy.GetComponent<EnemyInfo>().GetEnemyLevel();

            // Sets up which health source we want for the health bar
            SetHealthSource(enemy);

            // Updates the health.
            SetHealth();
        }
    }
}