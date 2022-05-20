using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


    public class PlayerNameInput : MonoBehaviour
    {
        [Header("UI")] 
        [SerializeField] private TMP_InputField nameInputField;

        [SerializeField] private Button continueButton;


        //Follow 4 rows sets, and gets the players colour and name, as well as set the string used in player-preferences
        public static string displayName { get; private set; }
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
        
        public void SetPlayerColour(Color playerColor)
        {
            continueButton.interactable = !string.IsNullOrEmpty(playerColor.ToString());
        }


        public void SavePlayerName()
        {
            displayName = nameInputField.text;
            PlayerPrefs.SetString(PlayerPrefsNameKey, displayName);
        
        }
        //Name is saved in playerpreferences.
        /*
        public void SavePlayerColour()
        {
            
            playerColour = Color.red;
            PlayerPrefs.SetString(PlayerPrefsNameKey, ColorUtility.ToHtmlStringRGB(playerColour));
        }
        */

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

