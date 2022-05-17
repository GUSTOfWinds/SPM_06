using System.Collections;
using UnityEngine;

public class GoalTextScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Wait(20));
    }

    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
