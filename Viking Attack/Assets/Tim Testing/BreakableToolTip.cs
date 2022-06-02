using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using ItemNamespace;
using Mirror;
using UnityEngine;

public class BreakableToolTip : MonoBehaviour
{
    [SerializeField] private GameObject[] players;
    [SerializeField] private string text;
    private GameObject tipText;

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }
        tipText = other.transform.Find("UI").gameObject.transform.Find("TipBreakable").gameObject;
        if (tipText.active == false)
        {
            tipText.SetActive(true);
            
        }
        

    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }
        tipText = other.transform.Find("UI").gameObject.transform.Find("TipBreakable").gameObject;
        tipText.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
