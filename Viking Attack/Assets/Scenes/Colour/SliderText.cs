using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Colour
{
    public  class  SliderText : MonoBehaviour{
 
        [SerializeField] private  TMP_InputField textComponent;
        [SerializeField] private  string colorComponent;
        [SerializeField] private  Slider slider;
        public int colValue;

        //sets value on awake if there is a colorvalue, if player hasn't set it's white
        private  void Awake()
        {
            colValue = PlayerPrefs.GetInt(colorComponent);
            textComponent.text = colValue.ToString();
            
            slider.value = colValue;
        }

        //FixedUpdate limits value between 0 and 255, otherwise changes value of slider and such to match each other.
        //TODO fixa så detta funkar bättre för att customiza armor
        private void FixedUpdate()
        {
            if (colValue > 255) colValue = 255;
            if (colValue <  0|| colValue == null || textComponent.text == null) colValue = 0;
        }
        
        //Saves chosen colour
        public void SaveColour()
        {
            PlayerPrefs.SetInt(colorComponent, colValue);
        }

        
        //Supposed to get the value of colour of R.G.B
        public  int GetColValue()
        {
            return colValue;
        }            
        //Supposed to set the value of colour of R.G.B
        private  void SetColValue(int toSet)
        {
            colValue = toSet;
        }
        //sets value in input field based on the value of slider
        public  void SetInputFieldValue()
        {
            colValue = (int)slider.value;

        }
        //Changes string value of input box to int
        public  void SetSliderValue()
        {
            colValue = int.Parse(textComponent.text);

        }
        
        
    }
}