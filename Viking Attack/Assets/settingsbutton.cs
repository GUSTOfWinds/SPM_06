using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settingsbutton : MonoBehaviour
{
    public Transform camera;
    public Animator settingsAnimator;
    

    public void clickedSettings()
    {
        settingsAnimator.SetBool("settingsopen", !settingsAnimator.GetBool("settingsopen"));

        if (settingsAnimator.GetBool("settingsopen"))
        {
            StartCoroutine(rotateAway());
        } else
        {
            StartCoroutine(rotateBack());
        }
}

    private IEnumerator rotateAway()
    {
        // fade from transparent to opaque

        


        // fade from opaque to transparent
        for (float i = 2f; i >= 0; i -= Time.fixedDeltaTime)
        {
            // set color with i as alpha
            camera.Rotate(Vector3.up, -.7f);
            yield return null;
        }

        
    }

    private IEnumerator rotateBack()
    {
        // fade from transparent to opaque




        // fade from opaque to transparent
        for (float i = 2f; i >= 0; i -= Time.fixedDeltaTime)
        {
            // set color with i as alpha
            camera.Rotate(Vector3.up, .7f);
            yield return null;
        }


    }


}
