using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using Image = Microsoft.Unity.VisualStudio.Editor.Image;

public class CharacterScreen : MonoBehaviour
{
    [SerializeField] private Image playerImage; // might delete, TODO we should perhaps add a sprite of the player
    [SerializeField] private Text playerName;
    [SerializeField] private Text playerLevelText;
    [SerializeField] private Text healthStatPoints;
    [SerializeField] private Text damageStatPoints;
    [SerializeField] private Text staminaStatPoints;
    [SerializeField] private Text availableStatPoints;
    [SerializeField]
    public Animator animator;
    private GlobalPlayerInfo globalPlayerInfo;


    // sets all the stats to what the globalplayerinfo contains
    public void OpenCharacterScreen()
    {
        playerName.text = globalPlayerInfo.GetName();
        playerLevelText.text = globalPlayerInfo.GetLevel().ToString();
        healthStatPoints.text = globalPlayerInfo.GetHealthStatPoints().ToString();
        damageStatPoints.text = globalPlayerInfo.GetDamageStatPoints().ToString();
        staminaStatPoints.text = globalPlayerInfo.GetStaminaStatPoints().ToString();
        availableStatPoints.text = globalPlayerInfo.GetStatPoints().ToString();
    }

    // Increases the damage stat of the player when the damage button is pressed
    public void IncreaseDamage()
    {
        
        if (globalPlayerInfo.GetStatPoints() < 1)
            return;
        animator.SetTrigger("IncDMG");
        globalPlayerInfo.IncreaseDamageStatPoints();
        damageStatPoints.text = globalPlayerInfo.GetDamageStatPoints().ToString();
        availableStatPoints.text = globalPlayerInfo.GetStatPoints().ToString();
    }
    
    // Increases the health stat of the player when the health button is pressed
    public void IncreaseHealth()
    {
        if (globalPlayerInfo.GetStatPoints() < 1)

            return;
        animator.SetTrigger("IncHP");
        globalPlayerInfo.IncreaseHealthStatPoints();
        healthStatPoints.text = globalPlayerInfo.GetHealthStatPoints().ToString();
        availableStatPoints.text = globalPlayerInfo.GetStatPoints().ToString();
    }
    // Increases the stamina stat of the player when the stamina button is pressed
    public void IncreaseStamina()
    {
        if (globalPlayerInfo.GetStatPoints() < 1)
            return;
        animator.SetTrigger("IncSTAM");
        globalPlayerInfo.IncreaseStaminaStatPoints();
        staminaStatPoints.text = globalPlayerInfo.GetStaminaStatPoints().ToString();
        availableStatPoints.text = globalPlayerInfo.GetStatPoints().ToString();
    }
    
}
