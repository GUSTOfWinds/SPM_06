using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{

    private bool bossIsDead;
    
    void OnTriggerEnter(Collider other)
    {
        if (bossIsDead)
        {
            SceneManager.LoadScene(1);
        }
        
    }
    public void DeadBoss(string name)
    {
        if(name == "Boss")
        {
            bossIsDead = true;
        }
        
    }
}
