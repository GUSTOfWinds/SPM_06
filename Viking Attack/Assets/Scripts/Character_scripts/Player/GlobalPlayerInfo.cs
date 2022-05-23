using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;


public class GlobalPlayerInfo : NetworkBehaviour
{
    /**
        /**
     * @author Martin Kings
     */
        [SerializeField] private Component healthBar;

        [SerializeField] private Component staminaBar;
        [SerializeField] private Component experienceBar;
        [SyncVar] [SerializeField] private string playerName;
        [SyncVar] [SerializeField] private Color skinColour;
        [SyncVar] [SerializeField] private float health;
        [SyncVar] [SerializeField] private float maxHealth;
        [SerializeField] private ItemBase[] items;
        [SyncVar] [SerializeField] private bool alive;
        [SyncVar] [SerializeField] private float stamina;
        [SyncVar] [SerializeField] private float maxStamina;
        [SyncVar] [SerializeField] private float experience;
        [SyncVar] [SerializeField] private int level;
        [SyncVar] [SerializeField] private float levelThreshold;
        [SyncVar] [SerializeField] private int availableStatpoints;
        [SyncVar] [SerializeField] private float damage;
        [SyncVar] [SerializeField] private int meatStackNumber;
        [SyncVar] [SerializeField] private int armorLevel;
        [SerializeField] private SkinnedMeshRenderer skinMesh;


        private void Awake()
        {
            alive = true;
            items = new ItemBase[5];
            damage = 100;
            health = 100;
            maxHealth = 100;
            stamina = 100;
            maxStamina = 120;
            healthBar = gameObject.transform.Find("UI").gameObject.transform.Find("Health_bar").gameObject.transform
                .Find("Health_bar_slider").gameObject.GetComponent<PlayerHealthBar>();
            staminaBar = gameObject.transform.Find("UI").gameObject.transform.Find("Stamina_bar").gameObject.transform
                .Find("Stamina_bar_slider").gameObject.GetComponent<PlayerStaminaBar>();
            experienceBar = gameObject.transform.Find("UI").gameObject.transform.Find("Experience_bar").gameObject.transform
                .Find("Experience_bar_slider").gameObject.GetComponent<PlayerExperienceBar>();
            experience = 0;
            levelThreshold = 60;
            availableStatpoints = 0;
            level = 1;
            playerName = PlayerPrefs.GetString("PlayerName");
            armorLevel = 0;
        }


        public void IncreaseArmorLevel(int increase)
        {
            armorLevel += increase;
        }
        private NetworkManagerLobby room;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private NetworkManagerLobby Room
        {
            get
            {
                if (room != null) return room;
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }

        private void Start()
        {
            if (isLocalPlayer)
            {
                skinMesh.material.SetColor(BaseColor, skinColour);
                CmdSetPlayerName(playerName);
                CmdSetSkinColour(skinColour);

            }
        }

        public void SetItemSlot(int index, ItemBase itemBase)
        {
            items[index] = itemBase;
        }

        // Gets called upon during game launch, the main menu sets the player name
        [Command]
        public void CmdSetPlayerName(string insertedName)
        {
            playerName = insertedName;
        }
        // Gets called upon during game launch, the main menu sets the player name
        [Command]
        public void CmdSetSkinColour(Color32 skinColour)
        {
            this.skinColour = skinColour;
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
        [Server]
        public void SetSkinColour(Color insertedColor)
        {
            skinColour = insertedColor;
        }

        // Returns the player name
        public string GetName()
        {
            return playerName;
        }

        // Returns the player skin color
        public Color GetSkinColor()
        {
            return skinColour;
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
                if (health + difference < 0)
                {
                    health = 0;
                }
                else
                {
                    health += difference;
                }
            }
            else if (health + difference > maxHealth)
            {
                health = maxHealth;
            }

            healthBar.GetComponent<PlayerHealthBar>().SetHealth(health);
            if (health <= 0)
            {
                // Used by PlayerActivateEnemyHealthBar class on player objects and by 
                // RespawnPanelHandler
                EventInfo playerDeathEvent = new PlayerDeathEventInfo
                {
                    EventUnitGo = gameObject
                };
                EventSystem.Current.FireEvent(playerDeathEvent);
                gameObject.GetComponent<KillPlayer>().PlayerRespawn();
            }
        }

        public void SetHealth(float hp)
        {
            health = hp;
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


        // Increases the players current experience, will reset it to 0 if the player reaches next level
        public void IncreaseExperience(float exp)
        {
            experience += exp;
            if (experience >= levelThreshold * (1.3 * level))
            {
                IncreaseLevel();
                experience = 0;
            }

            experienceBar.GetComponent<PlayerExperienceBar>().SetExperience(experience);
        }

        public void IncreaseLevel()
        {
            EventInfo playerLevelUpInfo = new PlayerLevelUpEventInfo
            {
                netID = gameObject.GetComponent<NetworkIdentity>().netId
            };
            EventSystem.Current.FireEvent(playerLevelUpInfo);
            level++;
            availableStatpoints += 3;
        }

        public float GetExperience()
        {
            return experience;
        }

        public int GetLevel()
        {
            return level;
        }

        public int GetStatPoints()
        {
            return availableStatpoints;
        }

        public void IncreaseDamageStatPoints()
        {
            damage += 6;
            availableStatpoints--;
        }

        public void IncreaseHealthStatPoints()
        {
            maxHealth += 10;
            availableStatpoints--;
            healthBar.GetComponent<PlayerHealthBar>().SetHealth(health);
        }

        public void IncreaseStaminaStatPoints()
        {
            maxStamina += 10;
            availableStatpoints--;
            staminaBar.GetComponent<PlayerStaminaBar>().SetStamina(stamina);
        }

        public float GetLevelThreshold()
        {
            return levelThreshold;
        }

        public float GetDamage()
        {
            return damage;
        }

        public void IncreaseMeatStackNumber()
        {
            meatStackNumber++;
        }

        public int GetMeatStackNumber()
        {
            return meatStackNumber;
        }

        public void DecreaseMeatStackNumber()
        {
            meatStackNumber--;
        }
        [Server]
        public void SetDisplayName(string playerName)
        {
            this.playerName = playerName;
        }

        public int GetArmorLevel()
        {
            return armorLevel;
        }

        public override void OnStartClient()
        {
            if (Room != null) Room.InGamePlayer.Add(this.gameObject);
        }
        public override void OnStopClient()
        {
            if (Room != null) Room.InGamePlayer.Remove(this.gameObject);
        }

    }
