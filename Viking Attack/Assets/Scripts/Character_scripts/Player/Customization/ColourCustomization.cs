using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Character_scripts.Player.Customization
{
    public class ColourCustomization : NetworkBehaviour

    {

    //[SerializeField] private List<MeshRenderer> ObjectsToChangeColour = new List<MeshRenderer>();
    [SerializeField] private SkinnedMeshRenderer ArmorColour;

    private Color32 color32;

    public override void OnStartClient()
    {
        
        ChangeColour();
    }

    private void ChangeColour()
    {

        byte red = (byte)PlayerPrefs.GetInt("redValue");
        byte green = (byte)PlayerPrefs.GetInt("greenValue");
        byte blue = (byte)PlayerPrefs.GetInt("blueValue");
        color32 = new Color32(r: red, g: green, b: blue, a: 255);
        
        ArmorColour.material.SetColor("_Colour", color32);
    }



    }
}
