using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerpAnim : MonoBehaviour
{
    private float timeup = 0;
    private float timedown = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeup += Time.deltaTime;
        if (timeup<2) {
            transform.localScale = Vector3.one * Mathfx.Berp(0f, 1f, timeup*3);
        }else
        {
            timedown -= Time.deltaTime*3;
            transform.localScale = Vector3.one * timedown;
        }
      

        if (timedown <0 )
        {
            gameObject.SetActive(false);
        }
            
        
    }

    void OnEnable()
    {
        timeup = 0;
        timedown = 1;
    }
}
