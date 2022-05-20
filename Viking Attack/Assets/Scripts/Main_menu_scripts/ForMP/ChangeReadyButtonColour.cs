using TMPro;
using UnityEngine;

public class ChangeReadyButtonColour : MonoBehaviour
{
    [SerializeField] private TMP_Text ButtonText;
    private bool ReadyStatus = false;
    
    public void ChangeColour()
    {

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
