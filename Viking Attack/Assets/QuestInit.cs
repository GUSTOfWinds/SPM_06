using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Enable", 4);
        
    }
    private void Enable()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    // Update is called once per frame

}
