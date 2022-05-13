using System;
using System.Globalization;
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

        private  void Awake()
        {
            colValue = PlayerPrefs.GetInt(colorComponent);
            textComponent.text = colValue.ToString();
            
            slider.value = colValue;
        }

        private void FixedUpdate()
        {
            if (colValue > 255) colValue = 255;
            if (colValue < 0) colValue = 0;
            textComponent.text = colValue.ToString();
            slider.value = colValue;
            PlayerPrefs.SetInt(colorComponent, colValue);
        }

        

        public  int GetColValue()
        {
            return colValue;
        }

        private  void SetColValue(int toSet)
        {
            colValue = toSet;
        }
        
        public  void SetInputFieldValue()
        {
            colValue = (int)slider.value;

        }
        
        public  void SetSliderValue()
        {
            colValue = int.Parse(textComponent.text);

        }
        
        
    }
}