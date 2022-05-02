using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperienceBar : MonoBehaviour
{
    public Slider experienceBar; // the slider 
    [SerializeField] private GlobalPlayerInfo globalPlayerInfo; // contains the global info of the current player
    public Text levelText;

    private void Start()
    {
        experienceBar.maxValue = (globalPlayerInfo.GetLevel() * globalPlayerInfo.levelThreshold * 1.3f);
        experienceBar.value = globalPlayerInfo.GetExperience();
        levelText.text = globalPlayerInfo.GetLevel().ToString();
    }

    // Updates the value of the slider to the players current stamina (will be called upon when attackacking, sprinting etc)
    public void SetExperience(float experience)
    {
        experienceBar.maxValue = (globalPlayerInfo.GetLevel() * globalPlayerInfo.levelThreshold * 1.3f);
        experienceBar.value = experience;
        levelText.text = globalPlayerInfo.GetLevel().ToString();
    }

    private void Update()
    {
        //globalPlayerInfo.UpdateStamina(globalPlayerInfo.stamina += 0.02f * Time.deltaTime);
    }
}
