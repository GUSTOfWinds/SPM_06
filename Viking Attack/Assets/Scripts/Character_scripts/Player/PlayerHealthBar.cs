using System;
using UnityEngine;
using UnityEngine.UI;


// Who to blame: Martin Kings


public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthBar; // the slider 
    [SerializeField] private GlobalPlayerInfo globalPlayerInfo; // contains the global info of the current player

    private void Awake()
    {
        healthBar = GetComponent<Slider>();
    }

    private void Start()
    {
        healthBar.maxValue = globalPlayerInfo.GetMaxHealth();
        healthBar.value = globalPlayerInfo.GetHealth();
    }

    // Updates the value of the slider to the players current health (will be called upon when being attacked, healed etc)
    public void SetHealth()
    {
        healthBar.maxValue = globalPlayerInfo.GetMaxHealth();
        healthBar.value = globalPlayerInfo.health;
    }
}