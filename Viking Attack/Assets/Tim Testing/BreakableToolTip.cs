using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BreakableToolTip : MonoBehaviour
{
    [SerializeField] private GameObject[] players;
    [SerializeField] private TMP_Text textOne;
    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

    }
    void OnTriggerEnter(Collider other)
    {
        
            return;
        
    }
        // Update is called once per frame
        void Update()
    {
        
    }
}
