using TMPro;
using UnityEngine;
/**
 * @author Victor Wikner
 */
public class ChangeReadyButtonColour : MonoBehaviour
{
    [SerializeField] private TMP_Text ButtonText;
    [SerializeField] private AudioClip clip;
    private bool ReadyStatus = false;
    [SerializeField] private AudioSource audioSource;
    
    //Changes the buttons colour when a player presses it
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
