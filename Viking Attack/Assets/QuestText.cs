using System.Collections;
using TMPro;
using UnityEngine;

public class QuestText : MonoBehaviour
{

    [SerializeField] private GameObject[] Quests;

    void Start()
    {
        Quests[1].GetComponentInChildren<UnityEngine.UI.Text>().text = "Make your way to the end of the path of the island";


    }



    public void FinishQuest(int i)
    {
        Quests[i].transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Image>().IsActive();
        return;
    }

    public string GetQuest(int i)
    {
        return Quests[i].ToString();
    }



}
