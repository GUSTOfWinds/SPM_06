using TMPro;
using UnityEngine;

public class ChangeReadyButtonColour : MonoBehaviour
{
    [SerializeField] private TMP_Text ButtonText;
    [SerializeField] private AudioClip clip;
    private bool ReadyStatus = false;
    [SerializeField] private AudioSource audioSource;
    
    
    public void ChangeColour()
    {
        audioSource.PlayOneShot(clip);

        if (ReadyStatus == false)
        {
            ButtonText.text = "<color=#9CFF8D>Ready </color>";
            ReadyStatus = true;
        }
        else
        {
            ButtonText.text = "<color=white>Ready </color>";
            ReadyStatus = false;

        }
        
    }
}
