using UnityEngine;

public class SaveOnPlayer : MonoBehaviour
{
    private GameObject saveManager;
    // Start is called before the first frame update
    void Start()
    {
        saveManager = GameObject.Find("SaveManager");
        //if we are choosed load game from meny. obtain data from file
        if (PlayerPrefs.GetString("isLoadFile") == "True")
        {
            saveManager.GetComponent<SaveScript>().LoadGame();
        }
    }


    public void OnSaveClickFc()
    {
        saveManager.GetComponent<SaveScript>().SetHost(gameObject);
        saveManager.GetComponent<SaveScript>().SaveGame();

    }

    public void OnLoadClickFc()
    {
        saveManager.GetComponent<SaveScript>().LoadGame();
    }


    public void OnExit()
    {
        Application.Quit();
    }
}
