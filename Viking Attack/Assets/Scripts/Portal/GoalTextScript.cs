using System.Collections;
using TMPro;
using UnityEngine;

public class GoalTextScript : MonoBehaviour
{

    [SerializeField] private TMP_Text textOne;
    [SerializeField] private TMP_Text textTwo;
    void Start()
    {
        textOne.text = "Find your way to the portal to progress to the second level";
        textTwo.text = "Along the way you will encounter enemies protecting the treasure guardian";
        StartCoroutine(Wait(5));
    }

    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    public void UpdateTextAndDisplay(string one, string two)
    {
        textOne.text = one;
        textTwo.text = two;
    }

    public string GetTextOne()
    {
        return textOne.text;
    }
    
    public string GetTextTwo()
    {
        return textTwo.text;
    }

}
