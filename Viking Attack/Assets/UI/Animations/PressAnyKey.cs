using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PressAnyKey : MonoBehaviour
{
    public GameObject menu;
    public Animator animator;
    private UnityEngine.UI.Text text;
    
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<UnityEngine.UI.Text>();
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey)
        {
            animator.SetBool("pressedAKey", true);
            Invoke("menutransition", 1.5f);
            
        }
    }
    void menutransition() { 
        menu.SetActive(true);
        
    }
}
