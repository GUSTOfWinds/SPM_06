using System.Collections;
using TMPro;
using UnityEngine;

public class QuestText : MonoBehaviour
{

    [SerializeField] private GameObject[] Quests;
    [SerializeField] private Animator animator;

    void Start()
    {

        Quests[0].GetComponentInChildren<UnityEngine.UI.Text>().text = "Follow the Path to the mountain";
        Quests[0].transform.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Image>().enabled = false;


    }



    public void FinishQuest(int i)
    {
        Quests[i].transform.GetChild(1).GetComponentInChildren<UnityEngine.UI.Image>().IsActive();

        return;
    }



    public void OnEnable()
    {
        animator.SetTrigger("questswoop");
    }

    public string GetQuest(int i)
    {
        return Quests[i].ToString();
    }



}
