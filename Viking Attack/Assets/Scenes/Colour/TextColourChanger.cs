using System;
using Main_menu_scripts.ForMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Colour
{
    public class TextColourChanger : MonoBehaviour
    {
        [SerializeField] public Slider redSlider;
        [SerializeField] public Slider blueSlider;
        [SerializeField] public Slider greenSlider;
        [SerializeField] private TMP_Text mixedColour;

        private float redVal;
        private float blueVal;
        private float greenVal;
        private Color32 color32;


        /*
        private void Awake()
        {
            GameObject.FindGameObjectWithTag("RSlider");
            GameObject.FindGameObjectWithTag("GSlider");
            GameObject.FindGameObjectWithTag("BSlider");
            color32 = GetColour();

        }
        */

        private Color32 GetColour()
        {
            byte r = (byte)PlayerPrefs.GetInt("redValue");
            byte g = (byte)PlayerPrefs.GetInt("greenValue");
            byte b = (byte)PlayerPrefs.GetInt("blueValue");
            return new Color32(r, g, b, 255);
        }

        
        private void FixedUpdate()
        {
            
            redVal = redSlider.value;
            blueVal = blueSlider.value;
            greenVal = greenSlider.value;
            color32 = new Color32((byte)redVal, (byte)greenVal, (byte)blueVal, 255);
            mixedColour.color = color32;
            SaveColour();

        }

        private void SaveColour()
        {
            PlayerPrefs.SetInt("redValue", (int)redVal);
            PlayerPrefs.GetInt("greenValue", (int) greenVal);
            PlayerPrefs.GetInt("blueValue", (int) blueVal);
        }

        public void UpdateOnChange()
        {
            redVal = redSlider.value;
            blueVal = blueSlider.value;
            greenVal = greenSlider.value;
            mixedColour.color = new Color(redVal, greenVal, blueVal, 255);
        }
    }
}
