using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Inventory_scripts;
using System.Collections.Generic;
using Mirror;

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
    [SerializeField] GameObject landingPanel;
    [SerializeField] GameObject loadPanel;

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
        SaveData savePlayer = new SaveData
        {
            hostName = theHost.GetComponent<GlobalPlayerInfo>().GetName()
        };
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Dictionary<int, bool> isSpritActiv = new Dictionary<int, bool>();
        Dictionary<String, System.Object> dataToSave = new Dictionary<String, System.Object>();
        string name;
        Debug.Log(players.Length);
        foreach (var t in players)
        {
            Debug.Log(t);
            name = t.GetComponent<GlobalPlayerInfo>().GetName();
            Debug.Log(name);
            GameObject[] inventorySprites = t.GetComponent<PlayerInventory>().getSprites();
            Debug.Log(inventorySprites.Length);
            for (int j = 0; j < inventorySprites.Length; j++)
            {
                isSpritActiv.Add(j, inventorySprites[j].activeSelf);
            }
            savePlayer.playerInventory.Add(name, isSpritActiv);
            dataToSave = t.GetComponent<GlobalPlayerInfo>().SaveData();
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
        //Calls when we are in game, and the hots name is already updated      
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + saveFileName, FileMode.Open);
            SaveData playerData = (SaveData)bf.Deserialize(file);
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var t in players)
            {
                var playerName = t.GetComponent<GlobalPlayerInfo>().GetName();
                if (playerName == hostName)
                {
                    t.GetComponent<GlobalPlayerInfo>().LoadData(playerData.hostData[hostName]);
                }
                else
                {
                    //if the player is not host but we have he/she's data
                    if (playerData.clients.Contains(playerName))
                    {
                        t.GetComponent<GlobalPlayerInfo>().LoadData(playerData.clientData[playerName]);
                    }
                }
                //set all inventory item sprite to false and reset them based on data file
                t.GetComponent<PlayerInventory>().refreshHotbar();
                for (int j =0; j< playerData.playerInventory[playerName].Count; j++)
                {

                    if (playerData.playerInventory[playerName][j])
                    {
                        // if the item was activ, set it to activ now
                        t.GetComponent<PlayerInventory>().UpdateHeldItem(j);
                    }
                }//for j all player inventory items
            }


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
            loadPanel.SetActive(false);
            // LandingPanel.SetActive(true);
            //only host has the right to save the data, when we load from main meny. the host should be the same as the file 
            PlayerPrefs.SetString("isLoadFile", "True");
            PlayerNameInput.displayName = playerData.hostName;
            PlayerPrefs.SetString("PlayerName", playerData.hostName);
            hostName = playerData.hostName;
            networkManager.StartHost();
            networkManager.GetComponent<NetworkManagerLobby>().GetLobbyRoom().OnStartAuthority();
           

        }
        else
        {
            Debug.LogError("There is no save data!");
        }
    }

    public bool IsLoadingFile()
    {
        return isLoadFromFile;
    }


}//class save controller



[Serializable]
public class SaveData
{
    public string hostName;
    public string inventory;
    public HashSet<string> clients = new HashSet<string>();
    public Dictionary<String, Dictionary<String, System.Object>> hostData = new Dictionary<string, Dictionary<string, object>>(); //all the information about the host
    public Dictionary<String, Dictionary<String, System.Object>> clientData = new Dictionary<string, Dictionary<string, object>>();// all th information about clients
    public Dictionary<string,Dictionary<int,bool>> playerInventory = new Dictionary<string,Dictionary<int,bool>>(); //player's name and the "isSpritActiv" 

}



