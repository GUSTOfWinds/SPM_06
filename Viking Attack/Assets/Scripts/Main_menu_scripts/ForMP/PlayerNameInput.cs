using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")] 
    [SerializeField] private TMP_InputField nameInputField;

    [SerializeField] private Button continueButton;
    
    
    public static string displayName { get; private set; }
    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start() => SetupInputField();
    
    private void SetupInputField(){
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) return;
        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
        nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    public void SetPlayerName(String playerName)
    {
        continueButton.interactable = string.IsNullOrEmpty(playerName);
    }

    public void SavePlayerName()
    {
        displayName = nameInputField.text;
        PlayerPrefs.SetString(PlayerPrefsNameKey, displayName);
        
    }

}
