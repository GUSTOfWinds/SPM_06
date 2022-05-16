using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleControlsScreen : MonoBehaviour
{


    [SerializeField] private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleControls()
    {
        Debug.Log("aaaaaaaaaaaaaaaaaaa");
        animator.SetBool("controls", true);
    }
}


