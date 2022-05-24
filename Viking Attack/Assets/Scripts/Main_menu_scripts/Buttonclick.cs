using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttonclick : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    // Start is called before the first frame update
    

    // Update is called once per frame
    public void clickSound()
    {
        audioSource.PlayOneShot(clip);
    }
}
