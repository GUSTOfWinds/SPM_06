
using DefaultNamespace;
using UnityEngine;


// WHO TO BLAME: Martin Kings

// Container for all player specifics, will add experience gained, HP, level, items owned etc...
public class GlobalPlayerInfo : MonoBehaviour
{
    public string playerName;
    public Color skinColor;
    public float health;
    public float maxHealth;
    public ItemBase[] items;
    public bool alive = true;
    public float stamina;
    public float maxStamina;
    public Component healthBar;
    public Component staminaBar;

    private void Awake()
    {
        health = 100;
        maxHealth = 100;
        stamina = 100;
        maxStamina = 100;
        healthBar = gameObject.transform.Find("UI").gameObject.transform.Find("Health_bar").gameObject.transform.Find("Health_bar_slider").gameObject.GetComponent<PlayerHealthBar>();
        staminaBar = gameObject.transform.Find("UI").gameObject.transform.Find("Stamina_bar").gameObject.transform.Find("Stamina_bar_slider").gameObject.GetComponent<PlayerStaminaBar>();

    }

    // Gets called upon during game launch, the main menu sets the player name
    public void SetPlayerName(string insertedName)
    {
        playerName = insertedName;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    // Gets called upon during game launch, the main menu sets the player skin color
    public void SetSkinColor(Color insertedColor)
    {
        skinColor = insertedColor;
    }

    // Returns the player name
    public string GetName()
    {
        return playerName;
    }

    // Returns the player skin color
    public Color GetSkinColor()
    {
        return skinColor;
    }
    
    // Checks if the player is alive
    public bool IsAlive()
    {
        return alive;
    }

    // Adds or reduces health
    public void UpdateHealth(float difference)
    {
        if (health + difference <= maxHealth)
        {
            health += difference;
        }
        else
        {
            health = maxHealth;
        }
        healthBar.GetComponent<PlayerHealthBar>().SetHealth(health);
        if (health <= 0) {
            gameObject.GetComponent<KillPlayer>().PlayerRespawn();
        }
    }

    // Returns the current stamina
    public float GetStamina()
    {
        return stamina;
    }

    // Returns the players max stamina
    public float GetMaxStamina()
    {
        return maxStamina;
    }
    
    // Adds or reduces stamina
    public void UpdateStamina(float difference)
    {
        if (stamina + difference <= maxStamina)
        {
            stamina += difference;
        }
        else
        {
            stamina = maxStamina;
        }
        staminaBar.GetComponent<PlayerStaminaBar>().SetStamina(stamina);
    }

}