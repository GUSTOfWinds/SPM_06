using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Inventory_scripts;
using System.Collections.Generic;
using Mirror;
using Event;
using ItemNamespace;


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
        
        SaveData savePlayer = new SaveData
        {
            hostName = theHost.GetComponent<GlobalPlayerInfo>().GetName()
        };
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Dictionary<int, bool> isSpritActiv = new Dictionary<int, bool>();
        Dictionary<String, System.Object> dataToSave = new Dictionary<String, System.Object>();
        //Get the key status (relic) save the key informtion with all players
        GameObject keyObj = GameObject.FindGameObjectWithTag("Key");
  
        string name;
       
        foreach (var t in players)
        {
            name = t.GetComponent<GlobalPlayerInfo>().GetName();
            GameObject[] inventorySprites = t.GetComponent<PlayerInventory>().GetSprites();
           
            for (int j = 0; j < inventorySprites.Length; j++)
            {
                isSpritActiv.Add(j, inventorySprites[j].activeSelf);
            }
            savePlayer.playerInventory.Add(name, isSpritActiv);
            dataToSave = t.GetComponent<GlobalPlayerInfo>().SaveData();
            if (name == savePlayer.hostName)
            {
                //Check out if the boss is dead and save it with host
                //if we have the  spear, the boss is dead, otherwise is not 
                if (isSpritActiv[1])
                {
                    dataToSave.Add("isBossDead", (System.Object)"True");
                }
                else
                {
                    dataToSave.Add("isBossDead", (System.Object)"False");
                }
                //Save the key information with host
                if (keyObj != null)
                {
                    dataToSave.Add("isKeyFound", (System.Object)"False");
                }
                else
                {
                    dataToSave.Add("isKeyFound", (System.Object)"True");
                }
                savePlayer.hostData.Add(name, dataToSave);
            }
            else
            {
                savePlayer.clientData.Add(name, dataToSave);
            }
        }


        Debug.Log(Application.persistentDataPath); //print the path   
        if (Directory.Exists(Application.persistentDataPath + saveFileName))//if we have a file there
        {
            Directory.Delete(Application.persistentDataPath + saveFileName);
        }
        BinaryFormatter bf = new BinaryFormatter();  // binär konvertering
        FileStream file = File.Create(Application.persistentDataPath + saveFileName);
        bf.Serialize(file, savePlayer);
        file.Close();
        // some UI test on the screen to tell the player that data has been saved 
        //theHost.GetComponent<SaveOnPlayer>().showSaveSuccessPanel();
        Debug.Log("Game data saved!");
    }

    public void LoadGame()
    {

        //TO DO 
        //return when no player is found
        if (File.Exists(Application.persistentDataPath + saveFileName))
        { //Calls when we are in game, and the hots name is already updated      
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + saveFileName, FileMode.Open);
            SaveData playerData = (SaveData)bf.Deserialize(file);
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            hostName = playerData.hostName;
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
                t.GetComponent<PlayerInventory>().RefreshHotbar();
                for (int j = 0; j < playerData.playerInventory[playerName].Count; j++)
                {
                    //TO DO dropItem 
                    //Add to item list if is already gained 

                    if (playerData.playerInventory[playerName][j])
                    {
                        //Add to item list if is already gained 
                        // if the item was activ, set it to activ now
                        //Maybe it should be item pick up event
                        EventInfo itemDropEventInfo = new ItemDropEventInfo
                        {
                            itemBase = t.GetComponent<PlayerInventory>().inventory[j]
                        };

                        EventSystem.Current.FireEvent(itemDropEventInfo);
                        t.GetComponent<PlayerInventory>().UpdateHeldItem(j);

              
                    }
                }//for j all player inventory items
            }
            //check if we have the key in the saving file, destory the key in the current scen
           if((string)playerData.hostData[hostName]["isKeyFound"] == "True")
            {
                Destroy(GameObject.FindGameObjectWithTag("Key"));
               
                //And tell the portal
            }


            if ((string)playerData.hostData[hostName]["isBossDead"] == "True")
            {
                //if we has killed the boss, find it and destory in this scen
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var e in enemies)
                {
                    if (e.GetComponent<EnemyInfo>().GetName() == "Boss")
                    {
                        //Maybe we can call death event here. 
                        NetworkServer.Destroy(e);
                    }
                }
            }

            file.Close();
            Debug.Log("Data is loaded");
        }
        else
        {
            Debug.LogError("There is no save data!");
        }
           

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
    public Dictionary<string, Dictionary<int, bool>> playerInventory = new Dictionary<string, Dictionary<int, bool>>(); //player's name and the "isSpritActiv" 

}



