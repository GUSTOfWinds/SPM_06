using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main_menu_scripts.ForMP
{
    public class PlayerNameInput : MonoBehaviour
    {
        [Header("UI")] 
        [SerializeField] private TMP_InputField nameInputField;

        [SerializeField] private Button continueButton;

        [SerializeField] private List<Button> colourButton = new List<Button>();

        public static string displayName { get; private set; }
        private const string PlayerPrefsNameKey = "PlayerName";
        public static Color playerColour { get; private set; }
        public const string PlayerColourKey = "PlayerColour";

        private void Start() => SetupInputField();
    
        private void SetupInputField(){
            if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) return;
            string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
            nameInputField.text = defaultName;

            SetPlayerName(defaultName);
        }

        public void SetPlayerName(String playerName)
        {
            continueButton.interactable = !string.IsNullOrEmpty(playerName);
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
        
        public void SavePlayerColour()
        {
            
            playerColour = Color.red;
            PlayerPrefs.SetString(PlayerPrefsNameKey, ColorUtility.ToHtmlStringRGB(playerColour));
        
        }

    }
}
