using System;
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


        /*
        private void Awake()
        {
            GameObject.FindGameObjectWithTag("RSlider");
            GameObject.FindGameObjectWithTag("GSlider");
            GameObject.FindGameObjectWithTag("BSlider");

        }
        */

        
        private void FixedUpdate()
        {
            redVal = redSlider.value;
            blueVal = blueSlider.value;
            greenVal = greenSlider.value;
            mixedColour.color = new Color32((byte)redVal, (byte)greenVal, (byte)blueVal, 255);
            
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