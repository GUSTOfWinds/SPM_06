using UnityEngine;

public class SaveOnPlayer : MonoBehaviour
{
    private GameObject saveManager;
    [SerializeField] GameObject saveSuccessPanel;
    [SerializeField] GameObject menyPanel;
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
        saveManager.GetComponent<SaveScript>().setHost(gameObject);
        saveManager.GetComponent<SaveScript>().SaveGame();
        // TO DO a new UI which is not a child to menyPnael 
        //if the data has been saved, show this panel
        //saveSuccessPanel.SetActive(true);
        //Invoke("hideSuccessPanel", 0.5f);
    }

    public void OnLoadClickFc()
    {
        saveManager.GetComponent<SaveScript>().LoadGame();
    }

    private void hideSuccessPanel()
    {
        //hide successpanel after 1s 
        saveSuccessPanel.SetActive(false);
    }
}
