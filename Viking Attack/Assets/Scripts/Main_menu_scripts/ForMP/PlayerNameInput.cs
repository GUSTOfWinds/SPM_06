using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Victor Wikner
 */
    public class PlayerNameInput : MonoBehaviour
    {
        [Header("UI")] 
        [SerializeField] private TMP_InputField nameInputField;

        [SerializeField] private Button continueButton;
        [SerializeField] private Slider red;
        [SerializeField] private Slider green;
        [SerializeField] private Slider blue;



        //Follow 4 rows sets, and gets the players colour and name, as well as set the string used in player-preferences
        public static string displayName { get; set; }
        private const string PlayerPrefsNameKey = "PlayerName";
        public static Color32 playerColour { get; private set; }

        private void Start()
        { 
            SetupInputField();
            nameInputField.characterLimit = 12;
        } 
        
        //checks status of name length to set if button is interactable or not, as the player must have a name.
        private void FixedUpdate()
        {
            if (nameInputField.text.Length > 0)
            {
                continueButton.interactable = true;
            }
        }
        
        // this method sets the name of a player if they've set a name previously, otherwise returns.
        private void SetupInputField(){
            if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) return;
            string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
            nameInputField.text = defaultName;

            SetPlayerName(defaultName);
        }
        //only activates button, name is saved in other method
        public void SetPlayerName(String playerName)
        {
            MakeButtonActive();
        }

        public void SavePlayerName()
        {
            displayName = nameInputField.text;
            PlayerPrefs.SetString(PlayerPrefsNameKey, displayName);
        //for saving file and load file
        PlayerPrefs.SetString("isLoadFile", "False");
            SavePlayerColour();
        
        }
        //Name is saved in playerpreferences.
        
        private void SavePlayerColour()
        {
            PlayerPrefs.SetInt("redValue", (int)red.value);
            PlayerPrefs.SetInt("greenValue", (int)green.value);
            PlayerPrefs.SetInt("blueValue", (int)blue.value);
            playerColour = new Color32((byte)red.value, (byte)green.value, (byte)blue.value, 255);
        }

        public void MakeButtonActive()
        {
            if (nameInputField.text != null || nameInputField.text != "")
            {
                continueButton.interactable = true;
            }
            else
            {
                continueButton.interactable = false;
            }
        }

    }

