using UnityEngine;
using UnityEngine.UI;
using Image = Microsoft.Unity.VisualStudio.Editor.Image;


public class CharacterScreen : MonoBehaviour
{
    /**
     * @author Martin Kings
     */
    [SerializeField] private Image playerImage; // might delete, TODO we should perhaps add a sprite of the player
    [SerializeField] private Text playerName;
    [SerializeField] private Text playerLevelText;
    [SerializeField] private Text healthStatPoints;
    [SerializeField] private Text damageStatPoints;
    [SerializeField] private Text staminaStatPoints;
    [SerializeField] private Text availableStatPoints;
    [SerializeField] private Text expToLevel;
    [SerializeField] private Text level;
    [SerializeField] private GlobalPlayerInfo globalPlayerInfo;

    public Animator animator;
    public Animator otherAnimator;


    // sets all the stats to what the globalplayerinfo contains
    public void OpenCharacterScreen()
    {
        playerName.text = globalPlayerInfo.GetName();
        playerLevelText.text = globalPlayerInfo.GetLevel().ToString();
        healthStatPoints.text = globalPlayerInfo.GetMaxHealth().ToString();
        damageStatPoints.text = globalPlayerInfo.GetDamage().ToString() + "%";
        staminaStatPoints.text = globalPlayerInfo.GetMaxStamina().ToString();
        availableStatPoints.text = globalPlayerInfo.GetStatPoints().ToString();
        expToLevel.text = (globalPlayerInfo.GetLevelThreshold() * globalPlayerInfo.GetLevel() * 1.3f -
                        globalPlayerInfo.GetExperience()).ToString();
        level.text = globalPlayerInfo.GetArmorLevel().ToString();

    }

    // Increases the damage stat of the player when the damage button is pressed
    public void IncreaseDamage()
    {
        if (globalPlayerInfo.GetStatPoints() < 1)
        {
           
            return;
        }
        if (globalPlayerInfo.GetStatPoints() == 1)
        {
            otherAnimator.SetBool("pointsavailable", false);
            
        }

        animator.SetTrigger("incDMG");
        globalPlayerInfo.IncreaseDamageStatPoints();
        damageStatPoints.text = globalPlayerInfo.GetDamage().ToString() + "%";
        availableStatPoints.text = globalPlayerInfo.GetStatPoints().ToString();
    }

    // Increases the health stat of the player when the health button is pressed
    public void IncreaseHealth()
    {
        if (globalPlayerInfo.GetStatPoints() < 1)
        {
             
            return;
        }
        if (globalPlayerInfo.GetStatPoints() == 1)
        {
            otherAnimator.SetBool("pointsavailable", false);

        }

        animator.SetTrigger("incHP");
        globalPlayerInfo.IncreaseHealthStatPoints();
        healthStatPoints.text = globalPlayerInfo.GetMaxHealth().ToString();
        availableStatPoints.text = globalPlayerInfo.GetStatPoints().ToString();
    }

    // Increases the stamina stat of the player when the stamina button is pressed
    public void IncreaseStamina()
    {
        if (globalPlayerInfo.GetStatPoints() < 1)
        {
            
            return;

        }
        if (globalPlayerInfo.GetStatPoints() == 1)
        {
            otherAnimator.SetBool("pointsavailable", false);

        }

        animator.SetTrigger("incSTAM");
        globalPlayerInfo.IncreaseStaminaStatPoints();
        staminaStatPoints.text = globalPlayerInfo.GetMaxStamina().ToString();
        availableStatPoints.text = globalPlayerInfo.GetStatPoints().ToString();
    }
}