using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Inventory_scripts;
using System.Collections.Generic;
using ItemNamespace;
using Mirror;
using Main_menu_scripts.ForMP;

public class SaveScript : NetworkBehaviour
{
    int intToSave;
    float floatToSave;
    bool boolToSave;
    private string saveFileName = "/playedData.dat";
    private PlayerInventory hostInventory;
    private bool isLoadFromFile;
    private GameObject theHost;
    private string hostName;
    [SerializeField] private NetworkManagerLobby networkManager;
    [Header("Panels")]
    [SerializeField] GameObject nameInputPanel;
    [SerializeField] GameObject LandingPanel;

    private void Start()
    {
        DontDestroyOnLoad(this);
      isLoadFromFile = false;
     
    }
    public void setLoadFromFile()
    {

    }
    public void setHost(GameObject obj)
    {
        theHost = obj;
    }

    public void SaveGame()
    {
        Debug.Log(theHost);   
        SaveData savePlayer = new SaveData();
        savePlayer.hostName = theHost.GetComponent<GlobalPlayerInfo>().GetName();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Dictionary<int, bool> isSpritActiv = new Dictionary<int, bool>();
        Dictionary<String, System.Object> dataToSave = new Dictionary<String, System.Object>();
        string name;
        Debug.Log(players.Length);
        for (int i = 0; i < players.Length; i++)
        {
            Debug.Log(players[i]);
             name = players[i].GetComponent<GlobalPlayerInfo>().GetName();
            Debug.Log(name);
            GameObject[] inventorySprites = players[i].GetComponent<PlayerInventory>().getSprites();
            Debug.Log(inventorySprites.Length);
            for (int j = 0; j < inventorySprites.Length; j++)
            {
                if (inventorySprites[j].activeSelf)
                {
                    //if the player own this item
                    isSpritActiv.Add(j, true);
                }
                else
                {
                    isSpritActiv.Add(j, false);
                }
            }
            savePlayer.playerInventory.Add(name, isSpritActiv);
           dataToSave = players[i].GetComponent<GlobalPlayerInfo>().SaveData();
            if(name == savePlayer.hostName)
            {
                Debug.Log("This is host");
                savePlayer.hostData.Add(name, dataToSave);
            }
            else
            {
                Debug.Log("This is client");
                savePlayer.clientData.Add(name, dataToSave);
            }

        }
        Debug.Log(Application.persistentDataPath); //print the path   
        if (Directory.Exists(Application.persistentDataPath + saveFileName))//if we have a file there
        {
            Directory.Delete(Application.persistentDataPath + saveFileName, true);
        }
        BinaryFormatter bf = new BinaryFormatter();  // binär konvertering
        FileStream file = File.Create(Application.persistentDataPath + saveFileName);
        bf.Serialize(file, savePlayer);
        file.Close();
        Debug.Log("Game data saved!");
    }

    public void LoadGame()
    {
        //Calls when we are in game, and the hots name is aldreay updated      
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + saveFileName, FileMode.Open);
            SaveData playerData = (SaveData)bf.Deserialize(file);
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            string playerName;
        for (int i = 0; i < players.Length; i++)
        {
            Debug.Log(players[i]);
            playerName = players[i].GetComponent<GlobalPlayerInfo>().GetName();
            if (playerName == hostName)
            {
                players[i].GetComponent<GlobalPlayerInfo>().LoadData(playerData.hostData[hostName]);
            }
            else
            {
                //if the player is not host but we have he/she's data
                if (playerData.clients.Contains(playerName))
                {
                    players[i].GetComponent<GlobalPlayerInfo>().LoadData(playerData.clientData[playerName]);
                }
            }
                //set all inventory item sprite to false and reset them based on data file
                players[i].GetComponent<PlayerInventory>().refreshHotbar();
                 for (int j =0; j< playerData.playerInventory[playerName].Count; j++)
                {
         
                    Debug.Log(playerData.playerInventory[playerName][j]);
                if (playerData.playerInventory[playerName][j])
                {
                   // if the item was activ, set it to activ now
                     players[i].GetComponent<PlayerInventory>().UpdateHeldItem(j);
                }
            }//for j all player inventory items
            }//for i all players


            file.Close();
            Debug.Log("Data is loaded");
  
    }


    public void OnMenuLoad()
    {
        isLoadFromFile = true;

        if (File.Exists(Application.persistentDataPath + saveFileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + saveFileName, FileMode.Open);
            SaveData playerData = (SaveData)bf.Deserialize(file);
            nameInputPanel.SetActive(false);
            // LandingPanel.SetActive(true);
            Debug.Log(playerData.hostName);
            //only host has the right to save the data, when we load from main meny. the host should be the same as the file 
            PlayerPrefs.SetString("isLoadFile", "True");
            PlayerNameInput.displayName = playerData.hostName;
            PlayerPrefs.SetString("PlayerName", playerData.hostName);
            hostName = playerData.hostName;
            networkManager.StartHost();
            networkManager.GetComponent<NetworkManagerLobby>().getLobbyRoom().OnStartAuthority();
           

        }
        else
        {
            Debug.LogError("There is no save data!");
        }
    }

    public bool isLoadingFile()
    {
        return isLoadFromFile;
    }


}//class save controller



[System.Serializable]
public class SaveData
{
    public string hostName;
    public string inventory;
    public HashSet<string> clients = new HashSet<string>();
    public Dictionary<String, Dictionary<String, System.Object>> hostData = new Dictionary<string, Dictionary<string, object>>(); //all the information about the host
    public Dictionary<String, Dictionary<String, System.Object>> clientData = new Dictionary<string, Dictionary<string, object>>();// all th information about clients
    public Dictionary<string,Dictionary<int,bool>> playerInventory = new Dictionary<string,Dictionary<int,bool>>(); //player's name and the "isSpritActiv" 

}



