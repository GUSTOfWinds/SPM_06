using UnityEngine;


public class PressAnyKey : MonoBehaviour
{
    public GameObject menu;
    public Animator animator;
    private UnityEngine.UI.Text text;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey)
        {
            animator.SetTrigger("pressedanykey");
            Invoke("menutransition", 1.5f);
            
        }
    }
    void menutransition() { 
        menu.SetActive(false);
        
    }
}
