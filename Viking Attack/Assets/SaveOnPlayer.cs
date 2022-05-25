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
    }

    public void OnLoadClickFc()
    {
        saveManager.GetComponent<SaveScript>().LoadGame();
    }
    public void showSaveSuccessPanel()
    {
        //if the data has been saved, show this panel
        menyPanel.SetActive(false);
        saveSuccessPanel.SetActive(true);
        Invoke("hideSuccessPanel", 1f);
    }
    private void hideSuccessPanel()
    {
        //hide successpanel after 1s 
        saveSuccessPanel?.SetActive(false);
    }
}
