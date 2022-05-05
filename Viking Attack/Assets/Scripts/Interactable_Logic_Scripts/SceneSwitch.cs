using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{

    public bool bossIsDead = true;
    
    void OnTriggerEnter(Collider other)
    {
        if (bossIsDead)
        {
            SceneManager.LoadScene(1);
        }
        
    }
}
