using System;
using ItemNamespace;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;


public class EnemyHealthBar : MonoBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private Slider healthBar; // the slider 
    [SerializeField] private GameObject healthSource; // the enemy gameobject
    private EnemyVitalController enemyVitalController;

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
        SetHealthSource(enemy);
        SetHealth();
    }
}